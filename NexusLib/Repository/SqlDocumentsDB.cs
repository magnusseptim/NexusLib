using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Linq;
using NexusLib.Tools;
using NexusLib.Repository.Interfaces;
using NexusLib.Tools.Interfaces;
using NexusLib.Model;

namespace NexusLib.Repository
{
    /// <summary>
    /// Stubb of SQL Client Azure ConsmosDB 
    /// </summary>
    public class SqlDocumentsDB : ISqlDocumentsDB
    {
        public RequestOptions DefaultRequestOptrions { get; private set; }
        public HashSet<DatabaseSchema> Databases { get; private set; }
        public string ActiveDatabaseID { get; private set; }
        public string ActiveCollection { get; private set; }

        public DocumentClient Client { get; }
        IStandardInvocator stdInvocator;
        NLog.Logger logger;


        public SqlDocumentsDB(IStandardInvocator standardInvocator, NLog.Logger logger, string endpointUrl, string primaryKey, int offerThroughput = 2500)
        {
            this.stdInvocator = standardInvocator;
            this.logger = logger;
            Client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            ActiveDatabaseID = Vault.DefaultActiveDBID;
            ActiveCollection = Vault.DefaultActiveColID;
            DefaultRequestOptrions = new RequestOptions() { OfferThroughput = offerThroughput };
        }

        public SqlDocumentsDB(NLog.Logger logger, string endpointUrl, string primaryKey, RequestOptions requestOptions)
        {
            this.logger = logger;
            Client = new DocumentClient(new Uri(endpointUrl), primaryKey);
            Databases = new HashSet<DatabaseSchema>();
            ActiveDatabaseID = Vault.DefaultActiveDBID; ;
            ActiveCollection = Vault.DefaultActiveColID;
            DefaultRequestOptrions = requestOptions;
        }

        /// <summary>
        /// Create database
        /// </summary>
        /// <param name="dbID">Database ID</param>
        /// <param name="resourceID">Resource ID</param>
        /// <returns></returns>
        public Task CreateDatabase(string dbID, string resourceID)
         => this.Client.CreateDatabaseIfNotExistsAsync(
              new Database()
              {
                  Id = dbID,
                  ResourceId = string.IsNullOrEmpty(resourceID) ? Guid.NewGuid().ToString() : resourceID
              }).ContinueWith((x) =>
              {
                  Databases.Add(new DatabaseSchema() { Database = x.Result });
                  ActiveDatabaseID = dbID;
              });

        /// <summary>
        /// Delete database using dbID
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns>Database deletion or logger error task</returns>
        public Task DeleteDatabase(string dbID)
        {
            Task returned;
            try
            {
                // Create delete database request...
                returned = this.Client.DeleteDatabaseAsync(
                    UriFactory.CreateDatabaseUri(string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID),
                    null);
            }
            catch (Exception ex)
            {
                // Or Logger task in case of failed
                returned = Task.Factory.StartNew(() =>
                {
                    logger.Error(ex, $"Database {dbID} delete failed");
                });
            }

            return returned;
        }

        /// <summary>
        /// Create document collection in specific database
        /// </summary>
        /// <param name="colId">Collection Identifier</param>
        /// <param name="partitionKeyPaths">Partition key path</param>
        /// <param name="dbID">Database ID. If null will be default</param>
        /// <param name="reqOptions">Request Options like Offer Throughput</param>
        /// <returns>Collection creation or logger error task</returns>
        public Task<BaseResponse<DocumentCollection>> CreateCollection(string colId, string partitionKeyPath, string dbID = null, RequestOptions reqOptions = null)
        {
            Task<BaseResponse<DocumentCollection>> returned = stdInvocator.GetDefaultRespone<DocumentCollection>();
            try
            {
                returned = stdInvocator.InvokeStandardThreadPoolAction<DocumentCollection>(() =>
                {
                    BaseResponse<DocumentCollection> doneCorrect = new BaseResponse<DocumentCollection>(true);
                    DocumentCollection colect = new DocumentCollection();
                    colect.Id = colId;
                    partitionKeyPath = !partitionKeyPath.Contains("/") ? "/" + partitionKeyPath : partitionKeyPath;
                    colect.PartitionKey.Paths.Add(partitionKeyPath);

                    return this.Client.CreateDocumentCollectionIfNotExistsAsync(
                               UriFactory.CreateDatabaseUri(string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID),
                                colect,
                                reqOptions ?? DefaultRequestOptrions);
                }).RunMultiThread();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Create Collection {colId} was failed");
            }

            return returned;
        }

        /// <summary>
        /// Delete document collection from 
        /// </summary>
        /// <param name="dbID">Database ID. If null will be default</param>
        /// <param name="colId">Collection ID. If null will be default</param>
        /// <returns>Collection deletion or logger error task</returns>
        public Task<BaseResponse<DocumentCollection>> DeleteCollection(string dbID, string colId)
        {
            Task<BaseResponse<DocumentCollection>> returned = stdInvocator.GetDefaultRespone<DocumentCollection>();
            try
            {
                returned = stdInvocator.InvokeStandardThreadPoolAction<DocumentCollection>(() =>
                {
                    return this.Client.DeleteDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(
                        string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                        string.IsNullOrEmpty(colId) ? ActiveDatabaseID : colId
                        ));
                }).RunMultiThread();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Delete Collection {colId} was failed");
            }

            return returned;
        }

        /// <summary>
        /// Create Document by using async request
        /// </summary>
        /// <param name="document">Document to insert</param>
        /// <param name="colId">Collection ID. If null will be default</param>
        /// <param name="dbID">Database ID. If null will be default</param>
        /// <returns>Document creation or logger error task</returns>
        public Task<BaseResponse<Document>> CreateDocumentAsync(object document, string colId = null, string dbID = null)
        {
            Task<BaseResponse<Document>> returned = stdInvocator.GetDefaultRespone<Document>();
            try
            {
                returned = stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
                {
                    return (Task<ResourceResponse<Document>>)this.Client.CreateDocumentAsync(
                                UriFactory.CreateDocumentCollectionUri(
                                    string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                                    string.IsNullOrEmpty(colId) ? ActiveCollection : colId
                            ), document);
                }).RunMultiThread();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Create Document type of { document.GetType().ToString()} failed");
            }
            return returned;
        }

        /// <summary>
        /// Create user by in choosed database
        /// </summary>
        /// <param name="userToAdd">User to add</param>
        /// <param name="dbID">Id of database</param>
        /// <param name="options">Request options</param>
        /// <returns></returns>
        public Task<BaseResponse<User>> CreateUserAsync(User userToAdd, string dbID = null, RequestOptions options = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<User>(() =>
            {
                return (Task<ResourceResponse<User>>)this.Client.CreateUserAsync(UriFactory.CreateDatabaseUri(dbID), userToAdd, options);
            }).RunMultiThread();
        }

        /// <summary>
        /// Delete Document by using async request
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="colId"></param>
        /// <param name="dbID"></param>
        /// <returns>Document deletion or logger error task</returns>
        public Task<BaseResponse<Document>> DeleteDocumentAsync(string documentId, string colId = null, string dbID = null)
        {
            Task<BaseResponse<Document>> returned = stdInvocator.GetDefaultRespone<Document>(); ;
            try
            {
                returned = stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
                {
                    return (Task<ResourceResponse<Document>>)this.Client.DeleteDocumentAsync(
                        UriFactory.CreateDocumentUri(
                            string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                            string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                            documentId));
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Delete Document { documentId } failed");
            }
            return returned;
        }


        /// <summary>
        /// Read document asynchronusly fromm choosed db/collection
        /// </summary>
        /// <param name="documentID">Document ID</param>
        /// <param name="partitionKey">Partition key definition in the form of a JSON path</param>
        /// <param name="colId">Collection ID</param>
        /// <param name="dbID">Database ID</param>
        /// <returns>Readed document or logger error task</returns>
        public Task<BaseResponse<Document>> ReadDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null)
        => stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
        {
            return (Task<ResourceResponse<Document>>)this.Client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri
                (
                    string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                    string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                    documentID
                ), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
        }).RunMultiThread();

        /// <summary>
        /// Update document asynchronusly
        /// </summary>
        /// <typeparam name="T">Type op updated document</typeparam>
        /// <param name="documentID">Updated document id</param>
        /// <param name="document">Updated document</param>
        /// <param name="colId">Collection id</param>
        /// <param name="dbID">Database id</param>
        /// <returns>Updated document</returns>
        public Task<BaseResponse<Document>> UpdateDocumentAsync<T>(string documentID, T document, string colId = null, string dbID = null)
        => stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
        {
            return (Task<ResourceResponse<Document>>)this.Client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri
                (
                    string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                    string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                    documentID
                ), document);
        }).RunMultiThread();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentID"></param>
        /// <param name="partitionKey"></param>
        /// <param name="colId"></param>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public Task<BaseResponse<Document>> DeleteDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null)
        => stdInvocator.InvokeStandardThreadPoolAction<Document>(() =>
        {
            return (Task<ResourceResponse<Document>>)this.Client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri
                (
                    string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                    string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                    documentID
                ), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
        }).RunMultiThread();

        public Task<BaseResponse<User>> ReplaceUserAsync(User userToAdd, string dbID = null, RequestOptions options = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<User>(() =>
            {
                return (Task<ResourceResponse<User>>)this.Client.ReplaceUserAsync(UriFactory.CreateDatabaseUri(dbID), userToAdd, options);
            }).RunMultiThread();
        }

        public Task<BaseResponse<User>> UpsertUserAsync(User userToAdd, string dbID = null, RequestOptions options = null)
        {
            return stdInvocator.InvokeStandardThreadPoolAction<User>(() =>
            {
                return (Task<ResourceResponse<User>>)this.Client.UpsertUserAsync(UriFactory.CreateDatabaseUri(dbID), userToAdd, options);
            }).RunMultiThread();
        }

        /// <summary>
        /// Adding attachment to existing document
        /// </summary>
        /// <param name="documentID">Document ID</param>
        /// <param name="attachment">Attachment object</param>
        /// <param name="colId">Collection id</param>
        /// <param name="dbID">Database id</param>
        /// <param name="options">Request options</param>
        /// <returns></returns>
        public Task<BaseResponse<Attachment>> AddAttachmentToExistingDocument(string documentID, object attachment, string colId = null, string dbID = null, RequestOptions options = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
            {
                return this.Client.CreateAttachmentAsync(
                      UriFactory.CreateDocumentUri(
                        string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                        string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                        documentID
                    ), attachment, options);
            }).RunMultiThread();

        /// <summary>
        /// Adding attachment to existing document
        /// </summary>
        /// <param name="uri">Uri of document</param>
        /// <param name="attachment">Attachment object, could be ex. big image</param>
        /// <param name="requestOptions">Request options</param>
        /// <param name="mediaOptions">Media options, as ex. content type</param>
        /// <returns></returns>
        public Task<BaseResponse<Attachment>> AddAttachmentToExistingDocument(Uri uri, System.IO.Stream attachment, RequestOptions requestOptions = null, MediaOptions mediaOptions = null)
          => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
          {
              return this.Client.CreateAttachmentAsync(uri, attachment, mediaOptions, requestOptions);
          }).RunMultiThread();

        public Task<BaseResponse<Attachment>> DeleteAttachmentAsync(Uri uri, RequestOptions requestOptions = null, MediaOptions mediaOptions = null)
          => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
          {
              return this.Client.DeleteAttachmentAsync(uri, requestOptions);
          }).RunMultiThread();

        public Task<BaseResponse<Attachment>> ReadAttachmentAsync(Uri uri, RequestOptions requestOptions = null, MediaOptions mediaOptions = null)
          => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
          {
              return this.Client.ReadAttachmentAsync(uri, requestOptions);
          }).RunMultiThread();

        public Task<FeedResponse<Attachment>> ReadAttachmentFeedAsyn(Uri uri, FeedOptions requestOptions = null)
          => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
          {
              return this.Client.ReadAttachmentFeedAsync(uri, requestOptions);
          }).RunMultiThread();

        public Task<BaseResponse<Attachment>> DeleteAttachmentAsync(Uri uri, object attachment, RequestOptions requestOptions = null, MediaOptions mediaOptions = null)
         => stdInvocator.InvokeStandardThreadPoolAction<Attachment>(() =>
         {
             return this.Client.UpsertAttachmentAsync(uri, attachment, requestOptions);
         }).RunMultiThread();

        /// <summary>
        /// Method used to create user permission
        /// </summary>
        /// <param name="uri">User uri</param>
        /// <param name="permission">Permission to add</param>
        /// <param name="requestOptions">Request options</param>
        /// <returns>Added permission or logger error task</returns>
        public Task<BaseResponse<Permission>> CreatePermissionAsync(Uri uri, Permission permission, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Permission>(() =>
            {
                return this.Client.CreatePermissionAsync(uri, permission, requestOptions);
            }).RunMultiThread();

        /// <summary>
        /// Method used to create user permission
        /// </summary>
        /// <param name="userLink">User link</param>
        /// <param name="permission">Permission to add</param>
        /// <param name="requestOptions">Request options</param>
        /// <returns>Added permission or logger error task</returns>
        public Task<BaseResponse<Permission>> CreatePermissionAsync(string userLink, Permission permission, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Permission>(() =>
            {
                return this.Client.CreatePermissionAsync(userLink, permission, requestOptions);
            }).RunMultiThread();

        public Task<BaseResponse<Permission>> DeletePermissionAsync(string userLink, RequestOptions requestOptions = null)
           => stdInvocator.InvokeStandardThreadPoolAction<Permission>(() =>
           {
               return this.Client.DeletePermissionAsync(userLink, requestOptions);
           }).RunMultiThread();

        public Task<BaseResponse<Permission>> DeletePermissionAsync(Uri uri, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Permission>(() =>
            {
                return this.Client.DeletePermissionAsync(uri, requestOptions);
            }).RunMultiThread();

        /// <summary>
        /// Method used to create Stored Procedure
        /// </summary>
        /// <param name="collectionLink">Collection link</param>
        /// <param name="storedProcedure">Stored Procedure</param>
        /// <param name="requestOptions">Request options</param>
        /// <returns>Stored procedure or logger error task</returns>
        public Task<BaseResponse<StoredProcedure>> CreateStoredProcedureAsync(string collectionLink, StoredProcedure storedProcedure, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<StoredProcedure>(() =>
            {
                return this.Client.CreateStoredProcedureAsync(collectionLink, storedProcedure, requestOptions);
            }).RunMultiThread();

        /// <summary>
        /// Method used to create Stored Procedure
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="storedProcedure">Stored Procedure</param>
        /// <param name="requestOptions">Request options</param>
        /// <returns>Stored procedure or logger error task</returns>
        public Task<BaseResponse<StoredProcedure>> CreateStoredProcedureAsync(Uri uri, StoredProcedure storedProcedure, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<StoredProcedure>(() =>
            {
                return this.Client.CreateStoredProcedureAsync(uri, storedProcedure, requestOptions);
            }).RunMultiThread();

        public Task<BaseResponse<StoredProcedure>> DeleteStoredProcedureAsync(Uri uri, RequestOptions requestOptions = null)
           => stdInvocator.InvokeStandardThreadPoolAction<StoredProcedure>(() =>
           {
               return this.Client.DeleteStoredProcedureAsync(uri, requestOptions);
           }).RunMultiThread();

        public Task<BaseResponse<Trigger>> CreateTriggerAsync(string collectionLink, Trigger trigger, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Trigger>(() =>
            {
                return this.Client.CreateTriggerAsync(collectionLink, trigger, requestOptions);
            }).RunMultiThread();

        public Task<BaseResponse<Trigger>> CreateTriggerAsync(Uri uri, Trigger trigger, RequestOptions requestOptions = null)
            => stdInvocator.InvokeStandardThreadPoolAction<Trigger>(() =>
            {
                return this.Client.CreateTriggerAsync(uri, trigger, requestOptions);
            }).RunMultiThread();

        public Task<BaseResponse<Trigger>> DeleteTriggerAsync(Uri uri, RequestOptions requestOptions = null)
           => stdInvocator.InvokeStandardThreadPoolAction<Trigger>(() =>
           {
               return this.Client.DeleteTriggerAsync(uri, requestOptions);
           }).RunMultiThread();

        public Task<BaseResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(Uri uri, UserDefinedFunction userDefinedFunction, RequestOptions requestOptions = null)
           => stdInvocator.InvokeStandardThreadPoolAction<UserDefinedFunction>(() =>
           {
               return this.Client.CreateUserDefinedFunctionAsync(uri, userDefinedFunction, requestOptions);
           }).RunMultiThread();

        public Task<BaseResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(string collectionLink, UserDefinedFunction userDefinedFunction, RequestOptions requestOptions = null)
           => stdInvocator.InvokeStandardThreadPoolAction<UserDefinedFunction>(() =>
           {
               return this.Client.CreateUserDefinedFunctionAsync(collectionLink, userDefinedFunction, requestOptions);
           }).RunMultiThread();

        public Task<BaseResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(Uri uri, RequestOptions requestOptions = null)
          => stdInvocator.InvokeStandardThreadPoolAction<UserDefinedFunction>(() =>
          {
              return this.Client.DeleteUserDefinedFunctionAsync(uri, requestOptions);
          }).RunMultiThread();

        public Task<IQueryable<object>> QueryPartitionedDocument(object document, string propertyName, object compareObject, string colId = null, string dbID = null)
            => Task.Run(() =>
            {
                return Client.CreateDocumentQuery<object>(UriFactory.CreateDocumentCollectionUri
                   (
                       string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                       string.IsNullOrEmpty(colId) ? ActiveCollection : colId
                   )).Where(x => x.GetType().GetProperty(propertyName).GetValue(document) == compareObject);
            });
        public IEnumerable<dynamic> ExecuteQuery(string collectionName, Func<dynamic, bool> where)
        {
            IQueryable<dynamic> Query = where == null ? this.Client.CreateDocumentQuery<dynamic>(
                                                        UriFactory.CreateDocumentCollectionUri(ActiveDatabaseID, collectionName))
                                                      : (IQueryable<dynamic>)this.Client.CreateDocumentQuery<dynamic>(
                                                        UriFactory.CreateDocumentCollectionUri(ActiveDatabaseID, collectionName)).Where(where);

            foreach (var entity in Query)
            {
                yield return entity;
            }
        }
        public IEnumerable<dynamic> ExecuteQuery(string collectionName)
            => ExecuteQuery(collectionName, null);




        /// <summary>
        /// Create Document URI
        /// </summary>
        /// <param name="documentID">Document ID</param>
        /// <param name="colId">Collection ID</param>
        /// <param name="dbID">DatabaseID</param>
        /// <returns>Uri or null if no document was found</returns>
        private Uri DocumentUriBuilder(string documentID, string colId = null, string dbID = null)
        {
            Uri returned;
            try
            {
                returned = UriFactory.CreateDocumentUri(
                                string.IsNullOrEmpty(dbID) ? ActiveDatabaseID : dbID,
                                string.IsNullOrEmpty(colId) ? ActiveCollection : colId,
                                documentID
                            );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Uri creation failed");
                returned = null;
            }
            return returned;
        }
    }
}
