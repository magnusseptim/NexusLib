using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Linq;

namespace NexusLib.Repository
{
    /// <summary>
    /// Stubb of SQL client Azure ConsmosDB 
    /// </summary>
    public class SqlDocumentsDB
    {
        public RequestOptions defaultRequestOptrions { get; private set; }
        public HashSet<DatabaseSchema> Databases  { get; private set; }
        public string ActiveDatabaseID { get; private set; }
        DocumentClient client;


        public SqlDocumentsDB(string endpointUrl, string primaryKey)
        {
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            ActiveDatabaseID = "db";
        }

        public SqlDocumentsDB(string endpointUrl, string primaryKey, RequestOptions requestOptions)
        {
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            ActiveDatabaseID = "db";
            defaultRequestOptrions = new RequestOptions() { OfferThroughput = 2500 };
        }

        public Task<BaseResponse<Database>> CreateDatabase(string dbID, string resourceID)
        {
            BaseResponse<Database> doneCorrect = new BaseResponse<Database>(true);
            try
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    var resourceResponse = await this.client.CreateDatabaseIfNotExistsAsync(
                        new Database()
                        {
                            Id = dbID,
                            ResourceId = string.IsNullOrEmpty(resourceID) ? Guid.NewGuid().ToString() : resourceID
                        });
                    doneCorrect.ResourceResponse = resourceResponse;
                    Databases.Add(new DatabaseSchema() { Database = doneCorrect.ResourceResponse.Resource });
                    ActiveDatabaseID = dbID;
                });
            }
            catch (Exception ex)
            {
                doneCorrect = new BaseResponse<Database>(false, ex.Message);
            }

            return Task.FromResult(doneCorrect);
        }

        public Task<BaseResponse<DocumentCollection>> CreateCollection(string colId, string paths, string dbID  = null, RequestOptions reqOptions = null)
        {
            BaseResponse<DocumentCollection> doneCorrect = new BaseResponse<DocumentCollection>(true);
            DocumentCollection colect = new DocumentCollection();
            colect.Id = colId;
            paths = !paths.Contains("/") ? "/" + paths : paths;
            colect.PartitionKey.Paths.Add(paths);

            try
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    var resourceResponse = await client.CreateDocumentCollectionIfNotExistsAsync(
                            UriFactory.CreateDatabaseUri(string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID),
                            colect,
                            reqOptions == null ? defaultRequestOptrions : reqOptions
                        );
                    Databases.First(x => x.Database.Id == dbID).Collections.Add(resourceResponse);
                });
            }
            catch (Exception ex)
            {
                doneCorrect = new BaseResponse<DocumentCollection>(false, ex.Message);
            }
            return Task.FromResult(doneCorrect);
        }
    }
}
