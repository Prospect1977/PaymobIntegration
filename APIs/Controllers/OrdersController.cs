using APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private ApplicationDbContext db;

        public OrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [HttpPost]
        [Route("NewOrder")]
        public async Task<IActionResult> NewOrder(OrderModel userOrder)
        {
            ResponseModel resp = new ResponseModel();


            var myOrder = new WebProject.Data.UserOrder
            {
                UserId = userOrder.UserId,
                ProductId = userOrder.ProductId,
                UnitPrice = userOrder.UnitPrice,
                Quantity = userOrder.Quantity,
                Payment = userOrder.Payment,
                NetPayment = 0,
                DataDate = DateTime.Now,
                Source = userOrder.Source,
                OrderId = userOrder.OrderId,
            };
            if (userOrder.TransactionId != null)//kiosk
            {
                myOrder.TransactionId = userOrder.TransactionId;
            }
            db.UserOrders.Add(myOrder);
            db.SaveChanges();
            resp.status = true;
            resp.data = myOrder.Id;
            return Ok(resp);

        }

        public class OrderModel
        {
            public string UserId { get; set; }
            public int ProductId { get; set; }
            public double UnitPrice { get; set; }
            public int Quantity { get; set; }
            public double Payment { get; set; }
            public string Source { get; set; }
            public int OrderId { get; set; }
            public int? TransactionId { get; set; }

        }
    }
}
