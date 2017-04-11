using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts.Exceptions
{
    public class ConnectorInstanceException : Exception
    {
        public ConnectorInstanceException(string message)
            : base(message)
        {
        }
    }
}
