using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace MyIntegratedApp.Controllers
{
    public class ExportController : Controller
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
    public class Export : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDbHelperService _dbHelperService;

        public Export(IConfiguration configuration, IDbHelperService dbHelperService)
        {
            _configuration = configuration;
            _dbHelperService = dbHelperService;
           
        }

        [HttpGet("export/{entityType}/{format}")]
        public async Task<IActionResult> ExportData(string entityType, string format)
        {
            try
            {
                // Determine which entity to export
                string flag = entityType.ToLower() switch
                {
                    "products" => "a",
                    "orders" => "b",
                    "suppliers" => "c",
                    _ => throw new ArgumentException("Invalid entity type")
                };

                var parameters = new DynamicParameters();
                parameters.Add("@flag", flag);
                var data = await _dbHelperService.ExecuteQuery<dynamic>("spa_exportdata", parameters, true);

                return format.ToLower() switch
                {
                    "excel" => await ExportToExcel(data, $"{entityType}.xlsx", entityType),
                    "csv" => await ExportToCsv(data, $"{entityType}.csv", entityType),
                    "pdf" => await ExportToPdf(data, $"{entityType}.pdf", entityType),
                    _ => BadRequest("Invalid format specified")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting data: {ex.Message}");
            }
        }

        private async Task<IActionResult> ExportToExcel(IEnumerable<dynamic> data, string fileName, string sheetName)
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add(sheetName);
            ws.Cells.LoadFromCollection(data, true);

            // Auto-fit columns for better Excel display
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task<IActionResult> ExportToCsv(IEnumerable<dynamic> data, string fileName, string entityType)
        {
            var sb = new StringBuilder();

            // Handle NULL values and format the response
            var firstRow = data.FirstOrDefault();
            if (firstRow == null)
            {
                return NotFound("No data available to export");
            }

            // Write headers
            var props = ((IDictionary<string, object>)firstRow).Keys;
            sb.AppendLine(string.Join(",", props));

            // Write data rows
            foreach (var row in data)
            {
                var dict = (IDictionary<string, object>)row;
                var values = props.Select(p =>
                {
                    var value = dict[p];
                    if (value == null) return string.Empty;

                    // Handle dates and special characters
                    if (value is DateTime date)
                        return date.ToString("yyyy-MM-dd");

                    // Escape commas and quotes in CSV
                    var str = value.ToString();
                    if (str.Contains(",") || str.Contains("\""))
                        return $"\"{str.Replace("\"", "\"\"")}\"";

                    return str;
                });
                sb.AppendLine(string.Join(",", values));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", fileName);
        }

        private async Task<IActionResult> ExportToPdf(IEnumerable<dynamic> data, string fileName, string title)
        {
            using var memoryStream = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // Add title
            var titleParagraph = new Paragraph(title, new (Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titleParagraph.Alignment = Element.ALIGN_CENTER;
            document.Add(titleParagraph);
            document.Add(new Paragraph("\n"));

            // Create PDF table
            var firstRow = data.FirstOrDefault();
            if (firstRow == null)
            {
                document.Add(new Paragraph("No data available"));
                document.Close();
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }

            var props = ((IDictionary<string, object>)firstRow).Keys;
            var table = new PdfPTable(props.Count())
            {
                WidthPercentage = 100,
                SpacingBefore = 10f,
                SpacingAfter = 10f
            };

            // Add headers
            foreach (var prop in props)
            {
                table.AddCell(new PdfPCell(new Phrase(prop,
                    new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD))));
            }

            // Add data rows
            foreach (var row in data)
            {
                var dict = (IDictionary<string, object>)row;
                foreach (var prop in props)
                {
                    var value = dict[prop]?.ToString() ?? string.Empty;
                    table.AddCell(new PdfPCell(new Phrase(value,
                        new Font(Font.FontFamily.HELVETICA, 8))));
                }
            }

            document.Add(table);
            document.Close();

            return File(memoryStream.ToArray(), "application/pdf", fileName);
        }

        [HttpGet("print/{entityType}")]
        public async Task<IActionResult> GetPrintData(string entityType)
        {
            try
            {
                string flag = entityType.ToLower() switch
                {
                    "products" => "a",
                    "orders" => "b",
                    "suppliers" => "c",
                    _ => throw new ArgumentException("Invalid entity type")
                };

                var parameters = new DynamicParameters();
                parameters.Add("@flag", flag);
                var data = await _dbHelperService.ExecuteQuery<dynamic>("spa_exportdata", parameters, true);

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}