using Ceql.Contracts.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ceql.Configuration
{
    public class CeqlSection : ICeqlSection
    {
        public ConnectionConfig[] Connections { get; set; }

        public IEnumerable<IConnectionConfig> GetConnections()
        {
            return Connections;
        }
    }
}
