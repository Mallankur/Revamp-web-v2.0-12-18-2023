using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories
{
    public class SQLConnetter : ISQLconnecterAnkurMall228
    {
        public readonly string _connectionString;

        public SQLConnetter(IOptions<SQLConnectorOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        /// <summary>
        /// Fectch All the Data Async @Ankur 
        /// kz = (1+e)ln(1-xa) -eaxa
        /// T  = T0 + ^Ha{XA}Fa0/(mcp)
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FetchAllSQLBatchDataAsyn(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids)
        {
            // Funcation name give  compiler error  Task<IEnumerable<string>> is not a itreayer interface !

           
            var jsonResults = new List<string>();
            const int batchSize = 100000;


            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                sqlCommand.Parameters.AddWithValue("@RDIds", rdids);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                await Task.Run(() => adapter.Fill(dataTable)).ConfigureAwait(false);

                int totalRows = dataTable.Rows.Count;
                int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                for (int i = 0; i < batches; i++)
                {
                    int startIndex = i * batchSize;
                    int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                    DataTable batchTable = dataTable.Clone();
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        batchTable.ImportRow(dataTable.Rows[j]);
                    }

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                  jsonResults.Add(jsonData);

                }
                    return jsonResults; 
            }
        }
        
        /// <summary>
        /// fe
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
            public IEnumerable<string> FetchAllSQLBatchData(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids)
        {

            const int batchSize = 200000;

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                sqlCommand.Parameters.AddWithValue("@RDIds", rdids);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);


                int totalRows = dataTable.Rows.Count; // 22
                int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                for (int i = 0; i < batches; i++)
                {
                    int startIndex = i * batchSize;
                    int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                    DataTable batchTable = dataTable.Clone();
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        batchTable.ImportRow(dataTable.Rows[j]);
                    }



                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);


                    yield return jsonData;
                }
            }
        }

        /// <summary>
        /// p/rho*g + v2/2g + z = c 
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public   IEnumerable<string> StreamAllSQLDataForMulticycle(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids)
        {
            using SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
            sqlCommand.Parameters.AddWithValue("@RDIds", rdids);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            List<string> columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            while (reader.Read())
            {
                object[] array = new object[reader.FieldCount];
                reader.GetValues(array);
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int i = 0; i < array.Length; i++)
                {
                    dictionary.Add(columnNames[i], array[i]);
                }

                yield return JsonConvert.SerializeObject(dictionary);
            }
        }

    }
}

