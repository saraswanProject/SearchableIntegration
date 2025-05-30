using MyIntegratedApp.Models;
using SearchableIntegration.Models;
using static MyIntegratedApp.Models.Product;

namespace MyIntegratedApp.Helpers
{
    public interface IDashboardService
    {
            Task<DashboardViewModel> GetDashboardDataAsync();
        
    }
}
