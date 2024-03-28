using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;

namespace WebProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        public OrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [Authorize]
        public IActionResult NewOrder(int Id)
        {
            var MyProduct = db.Products.FirstOrDefault(m => m.Id == Id);
            return View(MyProduct);
        }
    }
}
