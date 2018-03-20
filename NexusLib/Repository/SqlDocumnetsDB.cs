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

namespace NexusLib.Repository
{
    /// <summary>
    /// Stubb of SQL client Azure ConsmosDB 
    /// </summary>
    public class SqlDocumnetsDB
    {
        DocumentClient client;

        private Func<object> Invoker { get; set; }
        protected string EndpointUrl { get; private set; }
        protected string PrimaryKey { get; private set; }


        public SqlDocumnetsDB(string endpointUrl, string primaryKey, IEnumerable<PropertyModel> propertyrModel)
        {
            this.EndpointUrl = endpointUrl;
            this.PrimaryKey = primaryKey;
        }

     
    }
}
