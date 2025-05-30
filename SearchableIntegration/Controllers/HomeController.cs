using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using Newtonsoft.Json;
using SearchableIntegration.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MyIntegratedApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbHelperService _dbHelperService;
        private readonly IConfiguration _configuration;
        private readonly IDashboardService _dashboardService;
        public HomeController(IConfiguration configuration, IDbHelperService dbHelperService, IDashboardService dashboardService )
        {
            _configuration = configuration;
            _dbHelperService = dbHelperService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();

            return View(dashboardData);
        }

        public IActionResult AccessDenied()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = User.Identity.Name;
            }
            return View();
        }

       

        private string GetMonthName(int month)
        {
            return new DateTime(2020, month, 1).ToString("MMM");
        }
    }
}