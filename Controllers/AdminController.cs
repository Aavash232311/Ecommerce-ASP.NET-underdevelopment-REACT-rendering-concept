using Microsoft.AspNetCore.Mvc;
using restaurant_franchise.Models;
using restaurant_franchise.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace restaurant_franchise.Controllers
{
    public class Cat
    {
        public string Category { get; set; } = string.Empty;
        public string relation { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("admin")]
    // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        public AuthDbContext _context;
        public AdminController(AuthDbContext op)
        {
            this._context = op;
        }

        [HttpPost]
        [Route("upload_cat")]
        public async Task<IActionResult> UploadCat(Cat val)
        {
            Category productCategory = new Category()
            {
                ProductCategory = val.Category
            };
            if (val.relation.Length != 0)
            {
                var Parent = _context.Categories.Where(x => x.Id == Guid.Parse(val.relation)).FirstOrDefault();
                if (Parent == null) return new JsonResult(BadRequest());
                productCategory.CategoryKey = Parent.Id;
                productCategory.Parent = Parent; // one child one parent
                Parent.Child.Add(productCategory);

            }
            var cat = _context.Categories.Add(productCategory);
            await _context.SaveChangesAsync();
            return new JsonResult(Ok());
        }
        [HttpGet]
        [Route("getMainCategory")]
        public IActionResult Pagination(string? parent)
        {
            if (parent != null && parent != "null")
            {
                return new JsonResult(_context.Categories.Where(x => x.CategoryKey == Guid.Parse(parent))
                .Select(p => new { p.Id, p.ProductCategory, p.createdAt }).Take(5).ToArray());
            }
            var main_category = _context.Categories.Where(x => x.Parent == null).Take(5).ToList();
            return new JsonResult(Ok(main_category));
        }

        [HttpGet]
        [Route("first_in_cat")] // https://localhost:7142/admin/first_in_cat?date=date below product &parent=null parent or child
        public IActionResult belowDate(DateTime date, string parent)
        {
            if (parent != "null")
            {
                try
                {
                    Guid parentId = Guid.Parse(parent);
                    var data = _context.Categories.Where(x => x.createdAt > date && x.CategoryKey == parentId)
                    .Select(p => new { p.Id, p.ProductCategory, p.createdAt }).Take(3).ToList();
                    return new JsonResult(Ok(data));
                }
                catch (Exception ex)
                {
                    return new JsonResult(BadRequest(ex));
                }
            }
            if (parent == "null")
            {
                var data = _context.Categories.Where(x => x.createdAt > date && x.Parent == null)
                .Select(p => new { p.Id, p.ProductCategory, p.createdAt }).Take(3).ToList();
                return new JsonResult(Ok(data));
            }
            return new JsonResult(Ok());
        }
        [HttpGet]
        [Route("searchCategory")]
        public IActionResult searchCategory(string parent, string query)
        {
            var category = _context.Categories
            .Where(x => x.ProductCategory.Contains(query) && x.CategoryKey == (parent == "0" ? null: Guid.Parse(parent))).ToList();
            return new JsonResult(Ok(category));
        }
    }
}