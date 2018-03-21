using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusLib.Model
{
    public class DatabaseSchema
    {
        public Database Database { get; internal set; }
        public HashSet<DocumentCollection> Collections { get; internal set; }
    }
}
