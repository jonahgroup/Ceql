using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Execution.Cache
{
    public class VirtualDataCacheReader : IVirtualDataReader
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        object IVirtualDataReader.this[int i]
        {
            get { return 0; }
        }

        object IVirtualDataReader.this[string c]
        {
            get { return 0; }
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
