using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Model
{
    public class SqlSnippet
    {

        private readonly string _text;

        public SqlSnippet(string text)
        {
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
