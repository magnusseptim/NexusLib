namespace NexusLib.Model
{
    public static class Vault
    {
        public const string BaseResponseErrMsg = "Done Correct";
        public const string DefaultActiveDBID = "testdb";
        public const string DefaultActiveColID = "defaultCol";
        public const string ConnectorConfFileName = "Nexus.CosmosDB.json";
        public const string ConnectorEndpointKey = "Endpoint";
        public const string ConnectorKeyKey = "Key";
        public const string ConnectorDBKey = "DatabaseName";
        public const string DbJsonInvalidData = "CosmosDB invalid json data";
        public const string DbEndpointEmpty = "CosmosDB endpoint is null or empty";

        public const string NexusApiConfFileName = "Nexus.NexusAPI.json";
        public const string SwaggerVersionKey = "Version";
        public const string SwaggerTitleKey = "Title";
        public const string SwaggerJsonPath = "SwaggerJsonPath";
        public const string SwaggerAPIName = "SwaggerAPIName";

        public class VSqlDocumentDB
        {
            public const string ErrorDeleteDatabaseMessage = "Database {0} delete failed";
            public const string ErrorCreateCollectionMessage = "Create Collection {0} was failed";
            public const string ErrorDeleteCollectionMessage = "Delete Collection {0} was failed";
            public const string ErrorCreateDocumentAsyncMessage = "Create Document type of {0} failed";
            public const string ErrorDeleteDocumentAsyncMessage = "Delete Document {0} failed";
            public const string ErrorDocumentUriBuilderMessage = "Uri creation failed";
        }

        public class VStandardInvocator
        {
            public const string ErrorGetDefaultResponeMessage = "No work was done";
        }

        public class VTypeBuilders
        {
            public const string GetPropertyPrefixText = "get_";
            public const string SetPropertyPrefixText = "set_";
        }

        public class VResultRetriever
        {
            public const string RetrieveResultAggrException = "Retrieving value from Task {0} failed";
        }
    }
}
