namespace MyIntegratedApp.Models
{
    public class ResponseModel
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public string status  { get; set; } = string.Empty;
    }
}