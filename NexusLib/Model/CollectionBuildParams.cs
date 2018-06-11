using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusLib.Model
{
    public class CollectionBuildParams
    {
        public CollectionBuildParams(string collectionId, string partitionKeyPath, string databaseID, RequestOptions requestOptions)
        {
            this.CollectionId = collectionId;
            this.PartitionKeyPath = partitionKeyPath;
            this.DatabaseID = databaseID;
            this.RequestOptions = requestOptions;
        }
        public string CollectionId { get; }
        public string PartitionKeyPath { get; }
        public string DatabaseID { get; }
        public RequestOptions RequestOptions { get; }
    }
}
