using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using SearchableIntegration.Models;

namespace MyIntegratedApp.Helpers
{
  

    public class DashboardService : IDashboardService
    {
        private readonly IDbHelperService _dbHelperService;
        private readonly IConfiguration _configuration;

        public DashboardService(IDbHelperService dbHelperService, IConfiguration configuration)
        {
            _dbHelperService = dbHelperService;
            _configuration = configuration;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            try
            {
                var parameters = new DynamicParameters();
                var multi = await _dbHelperService.ExecuteQueryMultipleAsync("spa_dashboard", parameters, true);

                // First result: Product by Category
                var productData = (await multi.ReadAsync()).Select(x => new ProductCategoryData
                {
                    Category = x.Category,
                    Count = x.Count,
                    TotalValue = x.TotalValue
                }).ToList();

                // Second result: Sales by Month
                var salesData = (await multi.ReadAsync()).Select(x => new SalesData
                {
                    Month = GetMonthName(x.Month),
                    Amount = x.Amount,
                    OrderCount = x.OrderCount
                }).ToList();

                // Third result: Order Status Distribution
                var orderStatusData = (await multi.ReadAsync()).Select(x => new OrderStatusData
                {
                    Status = x.Status,
                    Count = x.Count,
                    TotalAmount = x.TotalAmount
                }).ToList();

                // Fourth result: User Statistics
                var userStats = await multi.ReadFirstOrDefaultAsync();
                var userData = new UserStatistics
                {
                    TotalUsers = userStats.TotalUsers,
                    ActiveUsers = userStats.ActiveUsers,
                    NewUsersThisMonth = userStats.NewUsersThisMonth,
                    UniqueCustomers = userStats.UniqueCustomers
                };

                return new DashboardViewModel
                {
                    ProductCategories = productData,
                    MonthlySales = salesData,
                    OrderStatusDistribution = orderStatusData,
                    UserStatistics = userData
                };
            }
            catch (Exception ex)
            {
                // Log error
                throw new ApplicationException("Error fetching dashboard data", ex);
            }
        }

        private string GetMonthName(int month)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }
    }

}