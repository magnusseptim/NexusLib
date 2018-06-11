using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using NexusLib.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusLib.Repository
{
    public class SequenceSqlDocumentDB
    {
        private ISqlDocumentsDB sqlDocumentsDB;
        public SequenceSqlDocumentDB(ISqlDocumentsDB sqlDocumentsDB)
        {
            this.sqlDocumentsDB = sqlDocumentsDB;
        }

        IList<Task<BaseResponse<DocumentCollection>>> CreateCollections(IEnumerable<CollectionBuildParams> collectionParams)
            => collectionParams.Select(x => sqlDocumentsDB.CreateCollection(x.CollectionId, x.PartitionKeyPath, x.DatabaseID, x.RequestOptions)).ToList();

        //IList<Task<BaseResponse<Document>>> CreateDocuments(IEnumerable<CollectionBuildParams> collectionParams)
        //    => 


    }
}
