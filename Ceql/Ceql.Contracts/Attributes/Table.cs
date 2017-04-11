namespace Ceql.Contracts.Attributes
{
    using System;

    public class Table : Attribute
    {
        public Table(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
