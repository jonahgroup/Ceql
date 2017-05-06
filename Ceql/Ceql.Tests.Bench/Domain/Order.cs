using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ceql.Contracts.Attributes;
using Ceql.Contracts;

namespace Ceql.Tests.Bench.Domain
{
    [Table("ORDER")]
    public class Order : ITable
    {
        public const string ORDER_ID = "ORDER_ID";
        public const string CREATE_TS = "CREATE_TS";
        public const string CUSTOMER_ID = "CUSTOMER_ID";
        public const string USER_ID = "USER_ID";

        [Field(ORDER_ID,"INTEGER")]
        [PrimaryKey]
        [AutoSequence]
        public int OrderId { get; set; }
        [Field(CREATE_TS, "TIMESTAMP")]
        public DateTime CreateTs { get; set; }
        [Field(CUSTOMER_ID, "INTEGER")]
        public int CustomerId { get; set; }
        [Field(USER_ID, "INTEGER")]
        public int? UserId { get; set; }
    }
}
