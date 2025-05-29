using Dapper;
using System.Data;

namespace MyIntegratedApp.Helpers
{
    public interface IDbHelperService : IDisposable
    {
        Task<IEnumerable<T>> ExecuteQuery<T>(string sql, DynamicParameters? param = null, bool isproc = false);
        void Execute(string sql, DynamicParameters? param = null, bool isproc = false);
        DataTable ConvertToDataTable<T>(IList<T> data);
        Task<T?> ExecuteSingle<T>(string sql, DynamicParameters? param = null, bool isproc = false);
    }
}