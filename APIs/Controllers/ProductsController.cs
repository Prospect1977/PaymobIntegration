using APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ProductsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        [Route("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var AllProducts = db.Products;
            ResponseModel resp = new ResponseModel { status = true, data = AllProducts };
            return Ok(resp);
        }
    }
}
