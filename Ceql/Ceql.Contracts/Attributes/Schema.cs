namespace Ceql.Contracts.Attributes
{
    using System;

    public class Schema : Attribute
    {
        public string Name { get; private set; }

        public Schema(string name)
        {
            Name = name;
        }
    }
}
