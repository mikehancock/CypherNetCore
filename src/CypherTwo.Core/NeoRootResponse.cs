using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherTwo.Core
{
    internal class NeoRootResponse
    {
        public NeoRootResponse(string data, string management)
        {
            Data = data;
            Management = management;
        }

        public string Data { get; private set; }
        public string Management { get; private set; }
    }

    internal class NeoDataRootResponse
    {
        public NeoDataRootResponse(string transaction, string neo4jVersion)
        {
            Transaction = transaction;
            Neo4JVersion = neo4jVersion;
        }

        public string Transaction { get; private set; }
        public string Neo4JVersion { get; private set; }
    }
}
