using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using MyIntegratedApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyIntegratedApp.Controllers
{
    public class DummyApiIntegrationController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DummyApiIntegration : ControllerBase
    {

        private IConfiguration _configuration;
        private IEmployeeService _employeeService;
        private IDbHelperService _dbHelperService;
        public DummyApiIntegration(IConfiguration configuration, IDbHelperService dbHelperService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }
        [HttpGet("getemployees")]
        public IActionResult GetEmployees()
        {
          EmployeeResultModel  employeeResult= _employeeService.DummyApiIntegration();

            var response = JsonConvert.SerializeObject(employeeResult);
            return Ok(response);
        }
    }
}
