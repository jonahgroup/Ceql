using Ceql.Contracts;
using Ceql.Contracts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Tests.Bench.Domain
{
    [Table("CUSTOMER")]
    public class Customer : ITable
    {
        public const string CUSTOMER_ID = "CUSTOMER_ID";
        public const string CUSTOMER_TYPE_ID = "CUSTOMER_TYPE_ID";
        public const string TYPE_CD = "TYPE_CD";
        public const string TYPE_DESC = "TYPE_DESC";
        public const string CREATE_TS = "CREATE_TS";


        [Field(CUSTOMER_ID, "INTEGER")]
        [PrimaryKey]
        [AutoSequence]
        public int CustomerId { get; set; }
        [Field(CREATE_TS, "TIMESTAMP")]
        public DateTime CreateTs { get; set; }
        [Field(CUSTOMER_TYPE_ID, "INTEGER")]
        public int? CustomerTypeId { get; set; }
        [Field(TYPE_CD, "VARCHAR")]
        public string TypeCd { get; set; }
        [Field(TYPE_DESC, "VARCHAR")]
        public string TypeDesc { get; set; }
    }
}
