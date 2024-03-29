using APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public PaymobController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Route("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(PaymobResponse Resp)
        {
            //  ResponseModel model = new ResponseModel();
            //  var Resp= JsonConvert.DeserializeObject<dynamic>(Response);
            var TransId = Resp.obj.id;
            int OrderId = Resp.obj.order.id;
            var amount_cents = Resp.obj.amount_cents;
            var pending = Resp.obj.pending;
            var dataDate = Resp.obj.created_at;

            String transId = null;
            try
            {
                transId = Resp.obj.data.migs_transaction.id;
            }
            catch
            {
            }

            try
            {
                var MyUserOrder = db.UserOrders.FirstOrDefault(m => m.OrderId == OrderId);


                if (MyUserOrder.Payment * 100 == amount_cents && pending == false)
                {
                    MyUserOrder.PaymentCompleted = true;
                    MyUserOrder.DataDate = dataDate;
                    MyUserOrder.NetPayment = MyUserOrder.Payment - (((MyUserOrder.Payment * 0.025) + 2) + ((MyUserOrder.Payment * 0.025) + 2) * 0.14);


                    if (MyUserOrder.Source == "kiosk")
                    {

                    }
                    else
                    {
                        MyUserOrder.TransactionId = TransId;
                    }
                    db.SaveChanges();
                }

            }
            catch
            {

            }

            return Ok();

        }
    }
}
