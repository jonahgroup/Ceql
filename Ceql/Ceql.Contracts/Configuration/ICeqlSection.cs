using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts.Configuration
{
    public interface ICeqlSection
    {
        IEnumerable<IConnectionConfig> GetConnections();
    }
}
