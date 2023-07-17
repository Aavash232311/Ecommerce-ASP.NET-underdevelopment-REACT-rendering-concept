using Microsoft.AspNetCore.Mvc;
using restaurant_franchise.Models;
using Microsoft.EntityFrameworkCore;
using restaurant_franchise.Data;
using restaurant_franchise.Services;
using Microsoft.AspNetCore.Authorization;
using restaurant_franchise.Controllers;
using Newtonsoft.Json;
using System.Globalization;

namespace restaurant_franchise.Controllers
{

    public class SellerFormData
    {
        public IFormFile ProfileImage { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public int PhoneNumber { get; set; }
        public string AboutBusiness { get; set; } = string.Empty;
        public string ShopAddress { get; set; } = string.Empty;
    }

    public class SearlizeProduct
    {
        public string name { get; set; } = string.Empty;
        public int discount_amount { get; set; } = 0;
        public int price { get; set; } = 0;
        public string description { get; set; } = string.Empty;
        public string discount_valid_date { get; set; } = string.Empty;
        public string product_condition { get; set; } = string.Empty;
        public string related_tags {get; set;} = string.Empty;
    }

    public class ImageKeyValue
    {
        public string name { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
    }

    public class Price
    {
        public int price { get; set; }
    }

    [Produces("application/json")]
    [Route("market")]
    [ApiController]

    public class MarketController : ControllerBase
    {
        public AuthDbContext _context;
        public MarketController(AuthDbContext context)
        {
            this._context = context;
        }

        [HttpPost]
        [Route("RegisterSeller")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetDataFromAnotherAPI()
        {
            try
            {
                string? data = Request.Form["stringData"];
                if (data == "") return new JsonResult(BadRequest("all the fields are required"));
                var additionalData = JsonConvert.DeserializeObject<SellerFormData>(data);
                var file = Request.Form.Files[0];
                float sizeInbYTE = (file.Length / 1024);
                double inmB = sizeInbYTE / 1024;
                if (inmB > 5) return new JsonResult(BadRequest("File should be less than 5 mb"));
                Tool.Username(Request.Headers["Authorization"], out string username);
                var user = _context.Users.Where(x => x.username == username).FirstOrDefault();
                if (user == null) return new JsonResult(BadRequest());
                Tool.validImages(file.FileName, out bool isValid, out string fileNameStamp);
                if (isValid == false) return new JsonResult(BadRequest("Not Supported File type"));
                var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "static\\shop_profile");
                if (!Directory.Exists(FilePath)) return new JsonResult(BadRequest());
                var path = Path.Combine(Directory.GetCurrentDirectory(), "static\\shop_profile", fileNameStamp);
                // create a file path
                try
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    // IF EVERYTHING TILL NOW WORKS
                    var NewSeller = new Seller()
                    {
                        ShopName = additionalData.ShopName,
                        ProfileImage = Path.Combine("static\\shop_profile", fileNameStamp),
                        AboutBusiness = additionalData.AboutBusiness,
                        ShopAddress = additionalData.ShopAddress,
                        PhoneNumber = additionalData.PhoneNumber,
                        User = user
                    };
                    this._context.Seller.Add(NewSeller);
                    // assign a seller role
                    var Roles = _context.Roles.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (Roles == null) return new JsonResult(BadRequest("Something went wrong"));
                    Roles.RoleId = 3;
                    await _context.SaveChangesAsync(); // new role saved
                    Utility utils = new Utility(_context);
                    string jwt = utils.CreateToken(user);
                    return new JsonResult(Ok(jwt));
                }
                catch (Exception ex)
                {
                    return new JsonResult(BadRequest(ex));
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest("All the field are required or something went wrong"));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [Route("add_product_form110")]
        public async Task<IActionResult> UploadForm()
        {
            try
            {
                string? data = Request.Form["key"];
                if (data == null) return new JsonResult(BadRequest("Someting went wrong"));
                var ObjectData = JsonConvert.DeserializeObject<SearlizeProduct>(data);
                if (ObjectData == null) return new JsonResult(BadRequest("Something went wrong"));
                var file = Request.Form.Files;
                List<ImageKeyValue> keyval = new List<ImageKeyValue>();

                Tool.Username(Request.Headers["Authorization"], out string username);
                var user = _context.Users.Where(x => x.username == username).FirstOrDefault();
                if (user == null) return new JsonResult(BadRequest());

                // check file size for every image
                foreach (var i in file)
                {
                    float sizeInbYTE = (i.Length / 1024);
                    double inmB = sizeInbYTE / 1024;
                    if (inmB > 5)
                    {
                        return new JsonResult(BadRequest("File size limit 5 mb"));
                    }
                    Tool.validImages(i.FileName, out bool isValid, out string fileNameStamp);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "static\\public_product_images");
                    if (!Directory.Exists(FilePath)) return new JsonResult(BadRequest("Sever error :("));
                    // creating a path
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "static\\public_product_images", fileNameStamp);
                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await i.CopyToAsync(stream); // saving in file stream
                            keyval.Add(new ImageKeyValue()
                            {
                                name = i.Name,
                                path = Path.Combine("static\\public_product_images\\" + fileNameStamp),
                            });
                        }
                    }

                    catch (Exception ex)
                    {
                        return new JsonResult(BadRequest(ex));
                    }
                    if (isValid == false) return new JsonResult(BadRequest("Unsupport format"));
                }
                var tag = _context.Categories.Where(x => x.Id == Guid.Parse(ObjectData.related_tags)).FirstOrDefault();
                if (tag == null) return new JsonResult(BadRequest("Anonymous object can't be sent"));
                var product = new Product()
                {
                    name = ObjectData.name,
                    discount_amount = (decimal)ObjectData.price * ((decimal)ObjectData.discount_amount / 100),
                    price = ObjectData.price,
                    description = ObjectData.description,
                    user = user,
                    main_image = pathFromArr(keyval, "main_image"),
                    cover_0 = pathFromArr(keyval, "cover_0"),
                    cover_1 = pathFromArr(keyval, "cover_1"),
                    cover_2 = pathFromArr(keyval, "cover_2"),
                    cover_3 = pathFromArr(keyval, "cover_3"),
                    cover_4 = pathFromArr(keyval, "cover_4"),
                    addedDate = DateTime.Now,
                    product_condition = ObjectData.product_condition,
                    related_tags = tag
                };
                if (ObjectData.discount_valid_date != "") {
                    DateTime.TryParseExact(ObjectData.discount_valid_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                    product.discount_valid_date = dateTime;
                }
                this._context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new JsonResult(Ok(tag));
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest(ex));
            }
        }
        public string pathFromArr(List<ImageKeyValue> arr, string name)
        {
            var path = arr.Where(x => x.name == name).FirstOrDefault();
            if (path == null) return "";
            return path.path;
        }

        // Fetch User Product sort by recently added and add pagination later
        [HttpGet]
        [Route("get_product")]
        public IActionResult GetProducts()
        {
            Tool.Username(Request.Headers["Authorization"], out string username);
            var products = _context.Products.Where(x => x.user.username == username).Take(10).ToArray();
            return new JsonResult(Ok(products));
        }
    }
}