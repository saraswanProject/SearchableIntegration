namespace MyIntegratedApp.Models
{
  
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int SupplierID { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; } // Nullable
        public string Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}