namespace SearchableIntegration.Models
{


   
    public class OrderModel
    {
        public OrderModel()
        {
            this.OrderDetails = new List<OrderDetailModel>();
        }

        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }
    }

    public class OrderDetailModel
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
