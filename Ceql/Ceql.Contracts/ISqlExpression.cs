using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts
{
    public interface ISqlExpression
    {
        string Sql { get; }
    }
}
