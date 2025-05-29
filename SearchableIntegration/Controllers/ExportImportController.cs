using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using MyIntegratedApp.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace MyIntegratedApp.Controllers
{
    public class ExportImportController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            // This returns Views/ProductApi/Index.cshtml
            return View();
        }
    }
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExportImport : ControllerBase
    {
        private IConfiguration _configuration;
        private IEmployeeService _employeeService;
        private IDbHelperService _dbHelperService;
        public ExportImport(IConfiguration configuration, IDbHelperService dbHelperService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }

        [HttpPost("upload")]
        public IActionResult Upload([FromBody] JsonElement productArray)
        {
            var validRows = new List<DynamicParameters>();
            var skippedRows = new List<object>();
            int rowIndex = 2; // header row + 1-based index

            foreach (var item in productArray.EnumerateArray())
            {
                try
                {
                    var productName = item.GetProperty("ProductName").GetString();
                    var category = item.GetProperty("Category").GetString();
                    var priceStr = item.GetProperty("Price").GetString();
                    var stockStr = item.GetProperty("StockQuantity").GetString();
                    var supplierIdStr = item.GetProperty("SupplierID").GetString();
                    var mfgDateStr = item.GetProperty("ManufacturedDate").GetString();

                    if (string.IsNullOrEmpty(productName) ||
                        string.IsNullOrEmpty(category) ||
                        string.IsNullOrEmpty(priceStr) ||
                        string.IsNullOrEmpty(stockStr) ||
                        string.IsNullOrEmpty(supplierIdStr) ||
                        string.IsNullOrEmpty(mfgDateStr))
                    {
                        skippedRows.Add(new { Row = rowIndex, Reason = "Missing required fields" });
                        rowIndex++;
                        continue;
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@flag", "i");
                    parameters.Add("@id", 0);
                    parameters.Add("@name", productName);
                    parameters.Add("@category", category);
                    parameters.Add("@price", decimal.Parse(priceStr));
                    parameters.Add("@stockQuantity", int.Parse(stockStr));
                    parameters.Add("@supplierId", int.Parse(supplierIdStr));
                    parameters.Add("@manufacturedDate", DateTime.Parse(mfgDateStr));
                    parameters.Add("@expiryDate", item.TryGetProperty("ExpiryDate", out var expiry) && !string.IsNullOrEmpty(expiry.ToString()) ? expiry.GetDateTime() : null);
                    parameters.Add("@description", item.TryGetProperty("Description", out var desc) ? desc.GetString() : "");

                    validRows.Add(parameters);
                }
                catch (Exception ex)
                {
                    skippedRows.Add(new { Row = rowIndex, Reason = ex.Message });
                }

                rowIndex++;
            }

            // ❌ If ANY row failed, reject the entire batch
            if (skippedRows.Any())
            {
                return BadRequest(new
                {
                    message = "Upload failed: some rows are invalid.",
                    skipped = skippedRows
                });
            }

            // ✅ All rows are valid — insert each
            foreach (var param in validRows)
            {
                _dbHelperService.ExecuteQuery<dynamic>("spa_exportImport", param, true);
            }

            return Ok(new
            {
                message = $"Successfully imported {validRows.Count} rows.",
                skipped = new List<object>() // empty
            });
        }

    }
}
