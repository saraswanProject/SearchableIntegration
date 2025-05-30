namespace SearchableIntegration.Models
{
   
        public class DashboardViewModel
        {
            public List<ProductCategoryData> ProductCategories { get; set; }
            public List<SalesData> MonthlySales { get; set; }
            public List<OrderStatusData> OrderStatusDistribution { get; set; }
            public UserStatistics UserStatistics { get; set; }
        }

        public class ProductCategoryData
        {
            public string Category { get; set; }
            public int Count { get; set; }
            public decimal TotalValue { get; set; }
        }

        public class SalesData
        {
            public string Month { get; set; }
            public decimal Amount { get; set; }
            public int OrderCount { get; set; }
        }

        public class OrderStatusData
        {
            public string Status { get; set; }
            public int Count { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public class UserStatistics
        {
            public int TotalUsers { get; set; }
            public int ActiveUsers { get; set; }
            public int NewUsersThisMonth { get; set; }
            public int UniqueCustomers { get; set; }
        }
    
}
