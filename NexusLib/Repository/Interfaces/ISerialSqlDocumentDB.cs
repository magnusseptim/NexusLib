using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusLib.Repository.Interfaces
{
    public interface ISerialSqlDocumentDB
    {
        IList<Task<BaseResponse<DocumentCollection>>> CreateCollections(IEnumerable<(string collectionId, string partitionKeyPath, string databaseID, RequestOptions requestOptions)> collectionParams);
        IList<Task<BaseResponse<DocumentCollection>>> DeleteCollections(IEnumerable<(string databaseID, string collectionID)> collectionParams);
        IList<Task<BaseResponse<Document>>> CreateDocuments(IEnumerable<(object document, string collectionID, string databaseID)> documentsParams);
        IList<Task<BaseResponse<Document>>> DeleteDocuments(IEnumerable<(string documentID, string collectionID, string databaseID)> documentsParams);
        IList<Task<BaseResponse<Document>>> ReadDocuments(IEnumerable<(string documentID, string partitionKey, string colId, string dbID)> documentParams);
        IList<Task<BaseResponse<Document>>> UpdateDocuments<DocumentType>(IEnumerable<(string documentID, DocumentType document, string colId, string dbID)> documentParams);
    }
}
