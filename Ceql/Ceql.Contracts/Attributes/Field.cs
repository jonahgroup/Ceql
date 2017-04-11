using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts.Attributes
{
    public class Field : Attribute
    {
        public Field(string fieldName)
        {
            Name = fieldName;
        }

        public Field(string fieldName, string type)
        {
            Name = fieldName;
            Type = type;
        }


        public string Name { get; private set; }
        public string Type { get; private set; }

    }
}
