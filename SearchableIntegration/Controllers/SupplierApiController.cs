using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using MyIntegratedApp.Models;
using SearchableIntegration.Models;
using System.Collections.Generic;

namespace MyIntegratedApp.Controllers
{
    public class SupplierApiController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }
    }
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierApi : ControllerBase
    {
        private IConfiguration _configuration;
        private IEmployeeService _employeeService;
        private IDbHelperService _dbHelperService;
        public SupplierApi(IConfiguration configuration, IDbHelperService dbHelperService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }
        [HttpGet("getsuppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "a");
            try
            {
                // Execute the stored procedure
                var suppliers = await _dbHelperService.ExecuteQuery<dynamic>("spa_suppliers", parameters, true);
                var response = suppliers.Select(p => new
                {
                    p.SupplierID,
                    p.SupplierName,
                    p.ContactNumber,
                    p.Email,
                    p.Address,
                    p.City,
                    p.Country
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
        public IActionResult Create([FromForm] SupplierModel supplier)
        {
         

            var parameters = new DynamicParameters();
            parameters.Add("@flag", "i");
            parameters.Add("@SupplierID", supplier.SupplierID);
            parameters.Add("@SupplierName", supplier.SupplierName);
            parameters.Add("@ContactNumber", supplier.ContactNumber);
            parameters.Add("@Email", supplier.Email);
            parameters.Add("@Address", supplier.Address);
            parameters.Add("@City", supplier.City);
            parameters.Add("@Country", supplier.Country);
       
            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_suppliers", parameters, true);
            var response = result.Result.Select(m => new
            {
                Message = m.Message,
                Code = m.Code,
            }).FirstOrDefault();
            return Ok(response);
        }
        [HttpPost("Update")]
        public IActionResult Update( [FromBody] SupplierModel supplier)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "u");
            parameters.Add("@SupplierID", supplier.SupplierID);
            parameters.Add("@SupplierName", supplier.SupplierName);
            parameters.Add("@ContactNumber", supplier.ContactNumber);
            parameters.Add("@Email", supplier.Email);
            parameters.Add("@Address", supplier.Address);
            parameters.Add("@City", supplier.City);
            parameters.Add("@Country", supplier.Country);

            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_suppliers", parameters, true);
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
            parameters.Add("@SupplierID", id);
            var result = _dbHelperService.ExecuteQuery<dynamic>("spa_suppliers", parameters, true);
            var response = result.Result.Select(m => new
            {
                Message = m.Message,
                Code = m.Code,
            }).FirstOrDefault();
            return Ok(response);
        }

     




    }
}
