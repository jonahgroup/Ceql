using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Execution
{
    public interface IVirtualDataReader : IDisposable
    {
        object this[int i] { get; }
        object this[string c] { get; }

        bool Read();
        void Close();
    }

}
