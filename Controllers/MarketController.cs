using Microsoft.AspNetCore.Mvc;
using restaurant_franchise.Models;
using Microsoft.EntityFrameworkCore;
using restaurant_franchise.Data;
using restaurant_franchise.Services;
using Microsoft.AspNetCore.Authorization;
using restaurant_franchise.Controllers;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;
using AutoMapper;
using System.Reflection;
using System.IO;

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
        public string related_tags { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
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
                            // appending saved file path for reference in database 
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
                    discount_amount = ObjectData.discount_amount,
                    price = ObjectData.price,
                    description = ObjectData.description,
                    user = user,
                    addedDate = DateTime.Now,
                    product_condition = ObjectData.product_condition,
                    related_tags = tag
                };

                if (ObjectData.discount_valid_date != "")
                {
                    DateTime.TryParseExact(ObjectData.discount_valid_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                    product.discount_valid_date = dateTime;
                }

                // ObjectData.type.length > 0 then its an update endpoint
                bool critetia = ObjectData.type.Length > 0;
                if (critetia == false)
                {
                    var path = keyval.Where(x => x.name == "main_image").FirstOrDefault(); // main image is required
                    if (path == null) return new JsonResult(BadRequest("Main IMAGE IS REQUIRED"));
                }
                // parent <- setting attribute dynamically and then that prouct object is gonna be copyed if
                // we are trying to upadte a model
                if (critetia == true)
                {
                    var getExistingProduct = _context.Products.Where(x => x.Id == Guid.Parse(ObjectData.type)).FirstOrDefault();
                    if (getExistingProduct == null) return new JsonResult(BadRequest());
                    // server defined function which copies to product to getExistingProduct
                    var reference = getExistingProduct;
                    Tool.CopyAttribute(product, getExistingProduct);
                    CopyImages(keyval, getExistingProduct, critetia);
                    getExistingProduct.Id = Guid.Parse(ObjectData.type); // copying does not automically assigns key
                    await _context.SaveChangesAsync();
                    return new JsonResult(Ok(getExistingProduct));
                }
                // work with image copying 
                if (keyval.Count != 0)
                {
                    CopyImages(keyval, product, critetia);
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new JsonResult(Ok());
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest(ex));
            }
        }

        public static void CopyImages(List<ImageKeyValue> arr, object obj, bool critetia)
        {
            foreach (var i in arr)
            {
                string name = i.name;
                DynamicAttribute(name, obj, critetia, arr); // if updating update the current model
            }
        }

        public static void DynamicAttribute(string name, object populatedObject, bool critetia, List<ImageKeyValue> arr)
        {
            var path = arr.Where(x => x.name == name).FirstOrDefault();
            Type type = populatedObject.GetType();
            PropertyInfo info = type.GetProperty(name);
            if (critetia == true)
            {
                if (path == null)
                {
                    return;
                }
            }
            if (path == null) return;
            // for whatever reason setting a value in case of updating remove old image first
            if (critetia) {
                // remove old image 
                var actualPath = Path.Combine(Directory.GetCurrentDirectory(), info.GetValue(populatedObject).ToString());
                if (System.IO.File.Exists((actualPath)))
                {
                    System.IO.File.Delete((actualPath));
                }
            }
            populatedObject.GetType().GetProperty(name).SetValue(populatedObject, path.path);
        }


        // Fetch User Product sort by recently added and add pagination later
        [HttpGet]
        [Route("get_product")]
        public IActionResult GetProducts()
        {
            Tool.Username(Request.Headers["Authorization"], out string username);
            var products = _context.Products.Where(x => x.user.username == username).Include(p => p.related_tags).Take(10).ToList();

            return new JsonResult(Ok(products));
        }
    }
}