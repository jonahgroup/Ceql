using Ceql.Contracts.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ceql.Configuration
{
    public class ConnectionConfig : IConnectionConfig
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Driver { get; set; }
        public bool Default { get; set; }
        public string Connector { get; set; }
        public string Formatter { get; set; }
    }
}
