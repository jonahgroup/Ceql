namespace Ceql.Connectors.MySql.Domain
{
    using System;
    using Ceql.Contracts;
    using Ceql.Contracts.Attributes;

    [Schema("INFORMATION_SCHEMA")]
    [Table("COLUMNS")]
    public class Columns : ITable
    {
        [Field("TABLE_CATALOG")]
        public string TableCatalog { get; set; }
        [Field("TABLE_SCHEMA")]
        public string TableSchema { get; set; }
        [Field("TABLE_NAME")]
        public string TableName { get; set; }
        [Field("COLUMN_NAME")]
        public string ColumnName { get; set; }
        [Field("ORDINAL_POSITION")]
        public ulong OrdinalPosition { get; set; }
        [Field("IS_NULLABLE")]
        public string IsNullable { get; set; }
        [Field("DATA_TYPE")]
        public string DataType { get; set;}
        [Field("COLUMN_TYPE")]
        public string ColumnType { get; set; }
        [Field("EXTRA")]
        public string Extra { get; set; }
        [Field("COLUMN_KEY")]
        public string ColumnKey { get; set; }
    }
}
