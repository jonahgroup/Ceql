using System;
using System.Collections.Generic;
using System.Text;

namespace Ceql.Contracts.Configuration
{
    public interface IConnectionConfig
    {
        string Name { get; }
        string ConnectionString { get; }
        string Driver { get; }
        string Connector { get; }
        string Formatter { get; }
        bool Default { get; }
    }
}
