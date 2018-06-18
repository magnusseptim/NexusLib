using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using NexusLib.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusLib.Repository
{
    public class SerialSqlDocumentDB : ISerialSqlDocumentDB
    {
        private ISqlDocumentsDB sqlDocumentsDB;
        public SerialSqlDocumentDB(ISqlDocumentsDB sqlDocumentsDB)
        {
            this.sqlDocumentsDB = sqlDocumentsDB;
        }

        public IList<Task<BaseResponse<DocumentCollection>>> CreateCollections(IEnumerable<(string collectionId, string partitionKeyPath, string databaseID, RequestOptions requestOptions)> collectionParams)
            => collectionParams.Select(x => sqlDocumentsDB.CreateCollection(x.collectionId, x.partitionKeyPath, x.databaseID, x.requestOptions)).ToList();

        public IList<Task<BaseResponse<DocumentCollection>>> DeleteCollections(IEnumerable<(string databaseID, string collectionID)> collectionParams) 
            => collectionParams.Select(x => sqlDocumentsDB.DeleteCollection(x.databaseID, x.collectionID)).ToList();

        public IList<Task<BaseResponse<Document>>> CreateDocuments(IEnumerable<(object document, string collectionID, string databaseID)> documentsParams)
            => documentsParams.Select(x => sqlDocumentsDB.CreateDocumentAsync(x.document, x.collectionID, x.databaseID)).ToList();

        public IList<Task<BaseResponse<Document>>> DeleteDocuments(IEnumerable<(string documentID, string collectionID , string databaseID)> documentsParams)
            => documentsParams.Select(x => sqlDocumentsDB.DeleteDocumentAsync(x.documentID, x.collectionID, x.databaseID)).ToList();

        public IList<Task<BaseResponse<Document>>> ReadDocuments(IEnumerable<(string documentID, string partitionKey, string colId, string dbID)> documentsParams)
            => documentsParams.Select(x => sqlDocumentsDB.ReadDocumentAsync(x.documentID, x.partitionKey, x.colId, x.dbID)).ToList();

        public IList<Task<BaseResponse<Document>>> UpdateDocuments<DocumentType>(IEnumerable<(string documentID, DocumentType document, string colId, string dbID)> documentsParams)
            => documentsParams.Select(x => sqlDocumentsDB.UpdateDocumentAsync(x.documentID, x.document, x.colId, x.dbID)).ToList();

        public IList<Task<BaseResponse<User>>> CreateUsersAsync(IEnumerable<(User userToAdd, string dbID, RequestOptions options)> usersParams)
            => usersParams.Select(x => sqlDocumentsDB.CreateUserAsync(x.userToAdd, x.dbID, x.options)).ToList();

        public IList<Task<BaseResponse<User>>> DeleteUserAsync(IEnumerable<(string dbID, string userId)> usersParams)
            => usersParams.Select(x => sqlDocumentsDB.DeleteUserAsync(x.dbID, x.userId)).ToList();
    
    }
}
