using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using NexusLib.Repository;
using NexusLib.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusLib.Tools
{
    public class SqlDocumentBuilder
    {
        private SqlDocumentsDB buildedObject;

        private IStandardInvocator standardInvocator;
        private NLog.Logger logger;
        private string endpointUrl;
        private string primaryKey;
        private int offerThroughput;
        private List<string> prefferedLocations;

        // TODO : Test this !!!
        public SqlDocumentsDB Return()
        {
            buildedObject.stdInvocator = standardInvocator;
            buildedObject.logger = logger;
            buildedObject.Client = new DocumentClient(new Uri(endpointUrl), primaryKey, BuildPrefferedLocationsPolicy(prefferedLocations));
            buildedObject.Databases = new HashSet<DatabaseSchema>();
            buildedObject.ActiveDatabaseID = Vault.DefaultActiveDBID;
            buildedObject.ActiveCollection = Vault.DefaultActiveColID;
            buildedObject.DefaultRequestOptrions = new RequestOptions() { OfferThroughput = offerThroughput };
            return buildedObject;
        }
        public static SqlDocumentBuilder BuildBareObject()
        {
            return new SqlDocumentBuilder();
        }

        public SqlDocumentBuilder StandardInvocator(IStandardInvocator sInvocator)
        {
            this.standardInvocator = sInvocator;
            return this;
        }

        public SqlDocumentBuilder Logger(NLog.Logger logger)
        {
            this.logger = logger;
            return this;
        }

        public SqlDocumentBuilder EndpointUrl(string endpointUrl)
        {
            this.endpointUrl = endpointUrl;
            return this;
        }

        public SqlDocumentBuilder PrimaryKey(string primaryKey)
        {
            this.primaryKey = primaryKey;
            return this;
        }

        public SqlDocumentBuilder OfferThroughput(int offerThroughput = 2500)
        {
            this.offerThroughput = offerThroughput;
            return this;
        }

        public SqlDocumentBuilder PrefferedLocations(List<string> prefferedLocations)
        {
            this.prefferedLocations = prefferedLocations;
            return this;
        }

        /// <summary>
        /// Allow to create connection policy with preffered locations based on parameter
        /// </summary>
        /// <param name="prefLocations">Preffered locations as string</param>
        /// <returns>Connection Policy</returns>
        private ConnectionPolicy BuildPrefferedLocationsPolicy(List<string> prefLocations)
        {
            var conPolicy = new ConnectionPolicy();
            prefferedLocations.ForEach(x => conPolicy.PreferredLocations.Add(x));
            return conPolicy;
        }     
    }
}
