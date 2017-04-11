using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts
{
    public interface ITransaction
    {
        void Execute();
        IEnumerable<T> Insert<T>(IEnumerable<T> records) where T : ITable;
    }
}
