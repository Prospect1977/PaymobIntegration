using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IHostingEnvironment _env;
        public ProductsController(ApplicationDbContext context, IHostingEnvironment env)
        {
            this.db = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return db.Products != null ?
                        View(await db.Products.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductsManager()
        {
            return db.Products != null ?
                        View(await db.Products.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }


        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult CreateProduct()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ProductName,Description,Price,PhotoUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UploadProductImage()
        {
            try
            {
                if (Request.Form.Files.Count != 0)
                {
                    var file = Request.Form.Files[0];
                    var path = Path.Combine(_env.WebRootPath, "images", "products");
                    return Ok(UploadImage(file, path));
                }
                else
                {
                    return BadRequest("Error!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public static string UploadImage(IFormFile file, string _path)
        {
            string uniqueFileName = null;
            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 5) + "_" + file.FileName;
            _path = Path.Combine(_path, uniqueFileName);

            using (var fs = new FileStream(_path, FileMode.Create))
            {
                file.CopyTo(fs);
            }

            return (uniqueFileName);
        }


    }
}
