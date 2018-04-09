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
using NexusLib.Tools;

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
        public string ActiveCollection { get; private set; }
        DocumentClient client;
        StandardInvocator stdInvocator;


        public SqlDocumentsDB(string endpointUrl, string primaryKey)
        {
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            stdInvocator = new StandardInvocator();
            ActiveDatabaseID = "db";
            ActiveCollection = "defaultCol";
            defaultRequestOptrions = new RequestOptions() { OfferThroughput = 2500 };
        }

        public SqlDocumentsDB(string endpointUrl, string primaryKey, RequestOptions requestOptions)
        {
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            ActiveDatabaseID = "db";
            ActiveCollection = "defaultCol";
            defaultRequestOptrions = new RequestOptions() { OfferThroughput = 2500 };
        }

        public Task CreateDatabase(string dbID, string resourceID)
        {

            try
            {
               return this.client.CreateDatabaseIfNotExistsAsync(
               new Database()
               {
                   Id = dbID,
                   ResourceId = string.IsNullOrEmpty(resourceID) ? Guid.NewGuid().ToString() : resourceID
               }).ContinueWith((x) =>
               {
                   Databases.Add(new DatabaseSchema() { Database = x.Result });
                   ActiveDatabaseID = dbID;
               });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<BaseResponse<DocumentCollection>> CreateCollection(string colId, string paths, string dbID  = null, RequestOptions reqOptions = null)
        {

            return stdInvocator.InvokeStandardThreadPoolAction<DocumentCollection>(() =>
                {
                    BaseResponse<DocumentCollection> doneCorrect = new BaseResponse<DocumentCollection>(true);
                    DocumentCollection colect = new DocumentCollection();
                    colect.Id = colId;
                    paths = !paths.Contains("/") ? "/" + paths : paths;
                    colect.PartitionKey.Paths.Add(paths);

                    return this.client.CreateDocumentCollectionIfNotExistsAsync(
                               UriFactory.CreateDatabaseUri(string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID),
                                colect,
                                reqOptions == null ? defaultRequestOptrions : reqOptions
                            );
                }).RunMultiThread();
          
        }

        public Task<BaseResponse<Document>> CreateDocumentAsync(object document, string colId = null, string dbID = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
            {
                return (Task<ResourceResponse<Document>>)this.client.CreateDocumentAsync(
                            UriFactory.CreateDocumentCollectionUri(
                                string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                                string.IsNullOrEmpty(colId) ? ActiveCollection : colId
                        ), document);
            }).RunMultiThread();
        }

        public Task<BaseResponse<Document>> ReadDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
            {
                return (Task<ResourceResponse<Document>>)this.client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri
                    (
                        string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                        string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                        documentID
                    ), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
            }).RunMultiThread();
        }

        public Task<BaseResponse<Document>> UpdateDocumentAsync<T>(string documentID, T document, string colId = null, string dbID = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
            {
                return (Task<ResourceResponse<Document>>)this.client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri
                    (
                        string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                        string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                        documentID
                    ), document);
            }).RunMultiThread();
        }

        public Task<BaseResponse<Document>> DeleteDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
            {
                return (Task<ResourceResponse<Document>>)this.client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri
                    (
                        string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                        string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                        documentID
                    ), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
            }).RunMultiThread();
        }

        public Task<IQueryable<object>> QueryPartitionedDocument(object document, string propertyName, object compareObject, string colId = null, string dbID = null)
        {
            return Task.Run(() =>
            {
                return client.CreateDocumentQuery<object>(UriFactory.CreateDocumentCollectionUri
                   (
                       string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                       string.IsNullOrEmpty(colId) ? ActiveCollection : colId
                   )).Where(x => x.GetType().GetProperty(propertyName).GetValue(document) == compareObject);
            });
        }
    }
}
