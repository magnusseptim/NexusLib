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
    }
}
