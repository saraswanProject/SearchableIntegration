namespace MyIntegratedApp.Models
{
    public class EmployeeResultModel
    {
        public EmployeeResultModel()
        {
            this.data = new List<EmployeeModel>();
        }
        public string status { get; set; }
        public string code { get; set; }
        public List<EmployeeModel> data { get; set; }

    }
    public class EmployeeModel
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public decimal employee_salary { get; set; }
        public int employee_age { get; set; }
        public string profile_image { get; set; }


    }

}