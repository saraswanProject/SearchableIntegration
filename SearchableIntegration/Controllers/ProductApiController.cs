using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using MyIntegratedApp.Models;
using System.Collections.Generic;

namespace MyIntegratedApp.Controllers
{
    public class ProductApiController : Controller
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Index()
        {
            // This returns Views/ProductApi/Index.cshtml
            return View();
        }
    }
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApi : ControllerBase
    {
        private IConfiguration _configuration;
        private IEmployeeService _employeeService;
        private IDbHelperService _dbHelperService;
        public ProductApi(IConfiguration configuration, IDbHelperService dbHelperService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }
        [HttpGet("getproducts")]
        public async Task<IActionResult> GetProducts()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "a");

            try
            {
                // Execute the stored procedure
                var products = await _dbHelperService.ExecuteQuery<dynamic>("spa_product", parameters, true);

                // Handle NULL values and format the response
                var response = products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Category,
                    p.Price,
                    p.StockQuantity,
                    p.SupplierName,
                    p.SupplierID,
                    ManufacturedDate = p.ManufacturedDate.ToString("yyyy-MM-dd"),
                    ExpiryDate = p.ExpiryDate?.ToString("yyyy-MM-dd") ?? "N/A",
                    Description = p.Description ?? string.Empty,
                    ImagePath = p.ImagePath ?? string.Empty
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while processing your request",
                    DetailedMessage = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }




        [HttpPost("Create")]
        public IActionResult Create([FromForm] Product product, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                if (!image.ContentType.StartsWith("image/")) return BadRequest("Invalid image type.");
                if (image.Length > 4 * 1024 * 1024) return BadRequest("Image size must be under 4MB.");

                var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
                var path = Path.Combine("wwwroot/images", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                image.CopyTo(stream);
                product.ImagePath = "/images/" + fileName;
            }

            var parameters = new DynamicParameters();
            parameters.Add("@flag", "i");
            parameters.Add("@id", product.Id);
            parameters.Add("@name", product.Name);
            parameters.Add("@category", product.Category);
            parameters.Add("@price", product.Price);
            parameters.Add("@stockQuantity", product.StockQuantity);
            parameters.Add("@supplierId", product.SupplierID);
            parameters.Add("@manufacturedDate", product.ManufacturedDate);
            parameters.Add("@expiryDate", product.ExpiryDate);
            parameters.Add("@description", product.Description);
            parameters.Add("@imagePath", product.ImagePath);

            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_product", parameters, true);
            var response = result.Result.Select(m => new
            {
                Message = m.Message,
                Code = m.Code,
            }).FirstOrDefault();
            return Ok(response);
        }
        [HttpPost("Update")]
        public IActionResult Update( [FromBody] Product updated)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "u");
            parameters.Add("@id", updated.Id);
            parameters.Add("@name", updated.Name);
            parameters.Add("@category", updated.Category);
            parameters.Add("@price", updated.Price);
            parameters.Add("@stockQuantity", updated.StockQuantity);
            parameters.Add("@supplierId", updated.SupplierID);
            parameters.Add("@manufacturedDate", updated.ManufacturedDate);
            parameters.Add("@expiryDate", updated.ExpiryDate);
            parameters.Add("@description", updated.Description);
            parameters.Add("@imagePath", updated.ImagePath);

            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_product", parameters, true);
            var response=result.Result.Select(m => new
            {
                Message=m.Message,
                Code=m.Code,
            }).FirstOrDefault();
            return Ok(response);
        }


        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "d");
            parameters.Add("@id", id);
            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_product", parameters, true);
            return Ok(result);
        }

        [HttpGet("getsuppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@flag", "s");
                var suppliers = await _dbHelperService.ExecuteQuery<dynamic>("spa_product", parameters, true);

                // Handle NULL values and format the response
                var response = suppliers.Select(p => new
                {
                    p.Id,
                    p.Name
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




    }
}
