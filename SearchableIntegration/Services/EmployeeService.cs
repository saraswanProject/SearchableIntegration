using MyIntegratedApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using MyIntegratedApp.Models;

namespace MyIntegratedApp.Helpers
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDbHelperService _dbHelperService;
        private IConfiguration _configuration;

        public EmployeeService(IDbHelperService dbHelperService,  IConfiguration configuration)
        {
            _configuration = configuration;
            _dbHelperService = dbHelperService;
        }

        public EmployeeResultModel DummyApiIntegration()
        {
            EmployeeResultModel responseModel = new EmployeeResultModel();
            try
            {
                string decisionStatus = string.Empty;
            
                // Prepare request URL and body
                string url = _configuration.GetValue<string>("DummyUrl");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();

                    // First API call
                    var request = new HttpRequestMessage(HttpMethod.Get, url)
                    {
                      //  Content = new StringContent(jsonstr, Encoding.UTF8, "application/json")
                    };


                    using (var response =  client.SendAsync(request))
                    {
                        string taskResponse = response.Result.Content.ReadAsStringAsync().Result;

                        if (!isValidJSON(taskResponse))
                        {
                            return new EmployeeResultModel
                            {
                                code = "404",
                                status = "Invalid Json Response"
                            };
                        }

                        responseModel = JsonConvert.DeserializeObject<EmployeeResultModel>(taskResponse);

                      
                    }
                }
            }
            catch (Exception ex)
            {
                return new EmployeeResultModel
                {
                    code = "9000",
                    status = "Technical Error: " + ex.Message
                };
            }

            return responseModel;
        }
        private bool isValidJSON(string json)
        {
            try
            {
                JObject jObject = JObject.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
