namespace Ceql.Connectors.MySql.Domain
{
    using System;
    using Ceql.Contracts;
    using Ceql.Contracts.Attributes;

    [Schema("INFORMATION_SCHEMA")]
    [Table("TABLES")]
    public class Tables : ITable
    {
        [Field("TABLE_CATALOG")]
        public string TableCatalog { get; set; }
        [Field("TABLE_SCHEMA")]
        public string TableSchema { get; set; }
        [Field("TABLE_NAME")]
        public string TableName { get; set; }
        [Field("TABLE_TYPE")]
        public string TableType { get; set; }
        [Field("ENGINE")]
        public string Engine { get; set; }
        [Field("VERSION")]
        public int Version { get; set; }
        [Field("ROW_FORMAT")]
        public string RowGormat { get; set; }
        [Field("TABLE_ROWS")]
        public ulong TableRows { get; set; }
        [Field("AVG_ROW_LENGTH")]
        public ulong AvgRowLength { get; set; }
        [Field("DATA_LENGTH")]
        public ulong DataLength { get; set; }
        [Field("MAX_DATA_LENGTH")]
        public ulong MaxDataLength { get; set; }
        [Field("INDEX_LENGTH")]
        public ulong IndexLength { get; set; }
        [Field("DATA_FREE")]
        public ulong DataFree { get; set; }
        [Field("AUTO_INCREMENT")]
        public ulong AutoIncrement { get; set; }
        [Field("CREATE_TIME")]
        public DateTime CreateTime { get; set; }
        [Field("UPDATE_TIME")]
        public DateTime UpdateTime { get; set; }
        [Field("CHECK_TIME")]
        public DateTime CheckTime { get; set; }
        [Field("TABLE_COLLATION")]
        public string TableCollation { get; set; }
        [Field("CHECKSUM")]
        public ulong CheckSum { get; set; }
        [Field("CREATE_OPTIONS")]
        public string CreateOptions { get; set; }
        [Field("TABLE_COMMENT")]
        public string TableComment { get; set; }
    }
}
