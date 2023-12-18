
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Revamp_Ank_App.Domain.Repositores.Entites;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using ThirdParty.Json.LitJson;

namespace Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories
{
    public class RevampRepositoryAnkurServises : ISQLconnecterAnkur
    {
        public IMongoCollection<RevampMongoDataModel> RevampCollection { get; set; }
        private readonly SQLConnetter _sqlConnetter;
        public RevampRepositoryAnkurServises(IOptions<MongoScoket> connect, SQLConnetter sql)
        {
            _sqlConnetter = sql;    
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(connect.Value.DatabaseName);
            RevampCollection = database.GetCollection<RevampMongoDataModel>(connect.Value.CollectionName);
        }

        /// <summary>
        /// ALp
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FetchAllSQLBatchProceesData(string storeProcedureName, int CycleId, string rdids)
        {


            var res = await _sqlConnetter.FetchAllSQLBatchDataAsyn(_sqlConnetter._connectionString, storeProcedureName, CycleId, rdids);
            return res;

         }

         
        


        public async Task<IEnumerable<string>> FetchAllSQLkafkaProcessing(string storeProcedureName, int CycleId, string rdids)
        {
            var multiCycleStreaming = _sqlConnetter.StreamAllSQLDataForMulticycle(_sqlConnetter._connectionString, storeProcedureName, CycleId, rdids);
            return multiCycleStreaming; 
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>

        public async Task<bool> CreateData_Using_SQL_SP_ConnectorAsync(string storeProcedureName, int CycleId, string rdids)
        {
            bool IsInseretd = false;


            if (CycleId == 3081 && rdids == "")
            {

                var sqlDataTabeleStreaming = await FetchAllSQLkafkaProcessing(storeProcedureName, CycleId, rdids);

                foreach (var jsonData in sqlDataTabeleStreaming)
                {
                    var stream_Documents = Newtonsoft.Json.JsonConvert.DeserializeObject<RevampMongoDataModel>(jsonData, new DateTimeToMillisecondConverter());

                    if (stream_Documents.ApplicationId.HasValue)
                    {

                        RevampCollection.InsertOneAsync(stream_Documents);
                        IsInseretd = true;
                    }
                }

                return IsInseretd;
            }


            else
            {
                var sqlDataTabeleResult = await FetchAllSQLBatchProceesData(storeProcedureName, CycleId, rdids);

                foreach (var Batchrow in sqlDataTabeleResult)
                {
                    IEnumerable<RevampMongoDataModel> revamapData = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<RevampMongoDataModel>>(Batchrow, new DateTimeToMillisecondConverter());
                    if (revamapData.Any())
                    {
                        RevampCollection.InsertManyAsync(revamapData);
                        IsInseretd = true;
                    }
                }

                return IsInseretd;
            }

                      return IsInseretd;
        }

        






            
            




        








        public Task<RevampMongoDataModel> GetRevamDataByRDIDSAsync(string[] rdids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RevampMongoDataModel> GetRevampRawDataAsync()
        {
            throw new NotImplementedException();
        }

        
    }
}
