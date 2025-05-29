using Dapper;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace MyIntegratedApp.Helpers
{
    public class DbHelperService : IDisposable, IDbHelperService
    {
        private readonly IConfiguration _configuration;
        public DbHelperService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private DbConnection GetConn()
        {
            DbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection;
        }
        public async Task<IEnumerable<T>> ExecuteQuery<T>(string sql, DynamicParameters? param = null, bool isproc = false)
        {
            DbConnection conn = GetConn();
            return await conn.QueryAsync<T>(sql, param, commandType: isproc ? CommandType.StoredProcedure : CommandType.Text);

        }

        public async Task<T?> ExecuteSingle<T>(string sql, DynamicParameters? param = null, bool isproc = false)
        {
            DbConnection conn = GetConn();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param, commandType: isproc ? CommandType.StoredProcedure : CommandType.Text);
        }


        public void Execute(string sql, DynamicParameters? param = null, bool isproc = false)
        {
            DbConnection conn = GetConn();
            conn.Execute(sql, param, commandType: isproc ? CommandType.StoredProcedure : CommandType.Text);
        }
        public void Dispose()
        {
            if (GetConn().State == ConnectionState.Open)
                GetConn().Close();
        }

        //public DataTable ConvertToDataTable<T>(IList<T> data)
        //{
        //    DataTable table = new DataTable();
        //    string json = JsonConvert.SerializeObject(data);
        //    table = (DataTable) JsonConvert.DeserializeObject(json, typeof(DataTable));
        //    return table;

        //}

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            if (data == null || data.Count == 0)
            {
                return new DataTable(); // Return empty table for null/empty input
            }

            // Get properties only once
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            // Create columns based on property types
            foreach (PropertyDescriptor prop in properties)
            {
                // Handle nullable types
                Type columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.Name, columnType);
            }

            // Add data rows
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    object value = prop.GetValue(item) ?? DBNull.Value;
                    row[prop.Name] = value;
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
