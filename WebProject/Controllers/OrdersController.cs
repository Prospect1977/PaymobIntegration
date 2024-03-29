using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private string fullName;
        private string phoneNumber;
        private string address;
        private string email;
        private string userId;
        private string firstToken;
        private string secondToken;
        private int orderId;
        private int userOrderId;
        private double payment;
        public OrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        [Authorize]
        public IActionResult NewOrder(int Id)
        {
            var MyProduct = db.Products.FirstOrDefault(m => m.Id == Id);
            return View(MyProduct);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NewOrder(int ProductId, int Quantity, string FullName, string PhoneNumber, string Address, double Payment)
        {
            fullName = FullName;
            phoneNumber = PhoneNumber;
            address = Address;
            email = User.Identity.Name;
            payment = Payment * 100;
            userId = db.Users.FirstOrDefault(m => m.Email == email).Id;
            var myOrder = new UserOrder
            {
                UserId = userId,
                ProductId = ProductId,
                UnitPrice = db.Products.FirstOrDefault(m => m.Id == ProductId).Price,
                Quantity = Quantity,
                Payment = Payment,
                NetPayment = 0,
                DataDate = DateTime.Now
            };
            db.UserOrders.Add(myOrder);
            db.SaveChanges();
            userOrderId = myOrder.Id;
            await SubmitOrderStep1();
            return Ok(secondToken);
        }
        public async Task<IActionResult> SubmitOrderStep1()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/auth/tokens");
            var content = new StringContent("{\r\n\"api_key\": \""+ContantsWeb.PaymobApiKey+"\"\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var firstResponse = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(firstResponse);
            firstToken = obj.token;
            await SubmitOrderStep2();
            return Ok();

        }
        public async Task<IActionResult> SubmitOrderStep2()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/ecommerce/orders");
            var content = new StringContent("{\r\n\"auth_token\":\"" + firstToken + "\", \r\n\"delivery_needed\": \"false\",\r\n\"amount_cents\":\"" + payment + "\",\r\n\"currency\": \"EGP\",\r\n\"items\":[]\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            
            var secondResponse = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(secondResponse);
            orderId = obj.id;
            var myOrder = db.UserOrders.FirstOrDefault(m => m.Id == userOrderId);
            myOrder.OrderId = orderId;
            db.SaveChangesAsync();
            await SubmitOrderStep3();
            return Ok();
        }
        public async Task<IActionResult> SubmitOrderStep3()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/payment_keys");
            var content = new StringContent("{\r\n\"auth_token\":\"" + firstToken + "\" ,\r\n\"amount_cents\":\"" + payment + "\",\r\n\"expiration\": 3600,\r\n\"order_id\":\"" + orderId + "\" ,\n\"billing_data\": {\r\n\"apartment\": \"NA\",\r\n\"email\":\"" + email + "\" ,\r\n\"floor\": \"NA\",\r\n\"first_name\":\"" + fullName.Split(" ")[0] + "\",\r\n\"street\": \"NA\",\r\n\"building\": \"NA\",\r\n\"phone_number\":\"" + phoneNumber + "\",\r\n\"shipping_method\": \"NA\",\r\n\"postal_code\": \"NA\",\r\n\"city\": \"NA\",\r\n\"country\": \"Egypt\",\r\n\"last_name\":\"" + fullName.Split(" ")[1] + "\" ,\r\n\"state\": \"NA\"\r\n},\r\n\"currency\": \"EGP\",\r\n\"integration_id\":\"" + ContantsWeb.IntegrationId + "\" , \n\"lock_order_when_paid\": \"false\",\r\n\"items\": []\r\n}\r\n\n", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var thirdResponse = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(thirdResponse);
            secondToken = obj.token;
            return Ok();
        }
    }
}
