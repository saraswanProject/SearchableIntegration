using Dapper;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using MyIntegratedApp.Models;
using OfficeOpenXml.Style;
using SearchableIntegration.Models;
using System.Data;


namespace MyIntegratedApp.Controllers
{
    public class OrderApiController : Controller
    {
        //[Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Index()
        {
            // This returns Views/ProductApi/Index.cshtml
            return View();
        }
    }
    //[Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApi : ControllerBase
    {
        private IConfiguration _configuration;
        private IEmployeeService _employeeService;
        private IDbHelperService _dbHelperService;
        public OrderApi(IConfiguration configuration, IDbHelperService dbHelperService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }
       

        [HttpGet("getorders")]
        public async Task<IActionResult> GetOrders(string fromDate = null, string toDate = null)
        {
            if (fromDate == null)
            {
                fromDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (toDate == null)
            {
                toDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "a");
            parameters.Add("@fromDate", fromDate);
            parameters.Add("@toDate", toDate);

            var orders = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);

       
            var response = orders.Select(x => new
            {
                x.OrderID,
                x.CustomerName,
                x.CustomerEmail,
                x.TotalAmount,
                x.OrderStatus,
                x.OrderDate

            }).ToList();

            return Ok(response);
        }

        [HttpGet("getorderdetails/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "od");
            parameters.Add("@orderId", orderId);
            var result = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);
            var response = result.Select(x => new
            {
                x.OrderDetailID,
                x.OrderID,
                x.ProductID,
                x.Quantity,
                x.SubTotal

            }).ToList();

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "i");
            parameters.Add("@OrderDate", order.OrderDate);
            parameters.Add("@CustomerName", order.CustomerName);
            parameters.Add("@CustomerEmail", order.CustomerEmail);
            parameters.Add("@TotalAmount", order.TotalAmount);
            parameters.Add("@OrderStatus", order.OrderStatus);
            var orderDetailsTable = ToOrderDetailsDataTable(order.OrderDetails);
            parameters.Add("@OrderDetails", orderDetailsTable.AsTableValuedParameter("dbo.OrderDetailsType"));


            var result = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);
            return Ok(result);
        }

        private static DataTable ToOrderDetailsDataTable(List<OrderDetailModel> orderDetails)
        {
            var table = new DataTable();
            table.Columns.Add("OrderID", typeof(int));
            table.Columns.Add("ProductID", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("SubTotal", typeof(decimal));

            foreach (var item in orderDetails)
            {
                table.Rows.Add(item.OrderID,item.ProductID, item.Quantity, item.SubTotal);
            }

            return table;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderModel order)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "u");
            parameters.Add("@OrderID", order.OrderID);
            parameters.Add("@CustomerName", order.CustomerName);
            parameters.Add("@CustomerEmail", order.CustomerEmail);
            parameters.Add("@OrderStatus", order.OrderStatus);
            parameters.Add("@TotalAmount", order.TotalAmount);
            var orderDetailsTable = ToOrderDetailsDataTable(order.OrderDetails);
            parameters.Add("@OrderDetails", orderDetailsTable.AsTableValuedParameter("dbo.OrderDetailsType"));

            var result = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@flag", "d");
            parameters.Add("@OrderID", id);

            var result = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);
            return Ok(result);
        }

        [HttpGet("getproducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@flag", "o");
                var suppliers = await _dbHelperService.ExecuteQuery<dynamic>("spa_orders", parameters, true);

                // Handle NULL values and format the response
                var response = suppliers.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price

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
