using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusLib.Repository.Interfaces
{
    public interface ISqlDocumentsDB
    {
        Task CreateDatabase(string dbID, string resourceID);
        Task DeleteDatabase(string dbID);
        Task<BaseResponse<DocumentCollection>> CreateCollection(string colId, string partitionKeyPath, string dbID = null, RequestOptions reqOptions = null);
        Task<BaseResponse<DocumentCollection>> DeleteCollection(string dbID, string colId);
        Task<BaseResponse<Document>> CreateDocumentAsync(object document, string colId = null, string dbID = null);
        Task<BaseResponse<Document>> DeleteDocumentAsync(string documentId, string colId = null, string dbID = null);
        Task<BaseResponse<Document>> ReadDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null);
        Task<BaseResponse<Document>> UpdateDocumentAsync<T>(string documentID, T document, string colId = null, string dbID = null);
        Task<BaseResponse<Document>> DeleteDocumentAsync(string documentID, string partitionKey, string colId = null, string dbID = null);
        Task<BaseResponse<User>> ReplaceUserAsync(User userToAdd, string dbID = null, RequestOptions options = null);
        Task<BaseResponse<User>> UpsertUserAsync(User userToAdd, string dbID = null, RequestOptions options = null);
        Task<BaseResponse<Attachment>> AddAttachmentToExistingDocument(string documentID, object attachment, string colId = null, string dbID = null, RequestOptions options = null);
        Task<BaseResponse<Attachment>> AddAttachmentToExistingDocument(Uri uri, System.IO.Stream attachment, RequestOptions requestOptions = null, MediaOptions mediaOptions = null);
        Task<BaseResponse<Attachment>> DeleteAttachmentAsync(Uri uri, RequestOptions requestOptions = null, MediaOptions mediaOptions = null);
        Task<BaseResponse<Attachment>> ReadAttachmentAsync(Uri uri, RequestOptions requestOptions = null, MediaOptions mediaOptions = null);
        Task<FeedResponse<Attachment>> ReadAttachmentFeedAsyn(Uri uri, FeedOptions requestOptions = null);
        Task<BaseResponse<Attachment>> DeleteAttachmentAsync(Uri uri, object attachment, RequestOptions requestOptions = null, MediaOptions mediaOptions = null);
        Task<BaseResponse<Permission>> CreatePermissionAsync(Uri uri, Permission permission, RequestOptions requestOptions = null);
        Task<BaseResponse<Permission>> CreatePermissionAsync(string userLink, Permission permission, RequestOptions requestOptions = null);
        Task<BaseResponse<Permission>> DeletePermissionAsync(string userLink, RequestOptions requestOptions = null);
        Task<BaseResponse<Permission>> DeletePermissionAsync(Uri uri, RequestOptions requestOptions = null);
        Task<BaseResponse<StoredProcedure>> CreateStoredProcedureAsync(string collectionLink, StoredProcedure storedProcedure, RequestOptions requestOptions = null);
        Task<BaseResponse<StoredProcedure>> CreateStoredProcedureAsync(Uri uri, StoredProcedure storedProcedure, RequestOptions requestOptions = null);
        Task<BaseResponse<StoredProcedure>> DeleteStoredProcedureAsync(Uri uri, RequestOptions requestOptions = null);
        Task<BaseResponse<Trigger>> CreateTriggerAsync(string collectionLink, Trigger trigger, RequestOptions requestOptions = null);
        Task<BaseResponse<Trigger>> CreateTriggerAsync(Uri uri, Trigger trigger, RequestOptions requestOptions = null);
        Task<BaseResponse<Trigger>> DeleteTriggerAsync(Uri uri, RequestOptions requestOptions = null);
        Task<BaseResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(Uri uri, UserDefinedFunction userDefinedFunction, RequestOptions requestOptions = null);
        Task<BaseResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(string collectionLink, UserDefinedFunction userDefinedFunction, RequestOptions requestOptions = null);
        Task<BaseResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(Uri uri, RequestOptions requestOptions = null);
        Task<IQueryable<object>> QueryPartitionedDocument(object document, string propertyName, object compareObject, string colId = null, string dbID = null);
        IEnumerable<dynamic> ExecuteQuery(string collectionName, Func<dynamic, bool> where);
        IEnumerable<dynamic> ExecuteQuery(string collectionName);
    }
}
