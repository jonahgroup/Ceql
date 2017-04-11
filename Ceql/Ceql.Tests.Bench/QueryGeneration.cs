namespace Ceql.Tests.Bench
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ceql.Tests.Bench.Domain;

    [TestClass]
    public class QueryGeneration : AbstractComposer
    {

        static QueryGeneration()
        {
            ConfigLoader.Load("mysql");
        }


        [TestMethod]
        public void SelectStar()
        {
            var select = From<Order>().Select(order => order);
            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0");
        }

        [TestMethod]
        public void SelectSome()
        {
            var select = From<Order>().Select(order => new
            {
                order.OrderId,
                order.CustomerId
            });

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.ORDER_ID T0_ORDER_ID,T0.CUSTOMER_ID T0_CUSTOMER_ID from `ORDER` T0");
        }

        [TestMethod]
        public void SelectWhere()
        {
            var select = From<Order>()
                .Where(order => order.CustomerId == 10)
                .Select(order => order);

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0 where T0.CUSTOMER_ID=10");
        }

        [TestMethod]
        public void SelectWithLimit()
        {
            var select = From<Order>()
                .Where(order => order.CustomerId == 10)
                .Select(order => order)
                .Limit(5);

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0 where T0.CUSTOMER_ID=10  limit 5");
        }

        [TestMethod]
        public void JoinInner2Tables()
        {
            var select = From<Order>()
                .Join<Customer>((o, c) => o.CustomerId == c.CustomerId)
                .Select((o, c) => new
                {
                    o.OrderId,
                    c.CustomerId,
                    c.CustomerTypeId,
                    c.TypeCd
                });

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.ORDER_ID T0_ORDER_ID,T1.CUSTOMER_ID T1_CUSTOMER_ID,T1.CUSTOMER_TYPE_ID T1_CUSTOMER_TYPE_ID,T1.TYPE_CD T1_TYPE_CD from `ORDER` T0 join `CUSTOMER` T1 on T0.CUSTOMER_ID=T1.CUSTOMER_ID");
        }

        [TestMethod]
        public void JoinInner2TablesWhere()
        {
            var select = From<Order>()
                .Join<Customer>((o, c) => o.CustomerId == c.CustomerId)
                .Where((o, c) => c.TypeCd == "NEW")
                .Select((o, c) => new
                {
                    o.OrderId,
                    c.CustomerId,
                    c.CustomerTypeId,
                    c.TypeCd
                });

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.ORDER_ID T0_ORDER_ID,T1.CUSTOMER_ID T1_CUSTOMER_ID,T1.CUSTOMER_TYPE_ID T1_CUSTOMER_TYPE_ID,T1.TYPE_CD T1_TYPE_CD from `ORDER` T0 join `CUSTOMER` T1 on T0.CUSTOMER_ID=T1.CUSTOMER_ID where T1.TYPE_CD='NEW'");
        }
    }
}
