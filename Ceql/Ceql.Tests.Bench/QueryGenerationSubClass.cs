namespace Ceql.Tests.Bench
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Order = Domain.OrderModel;
    using Customer = Domain.CustomerModel;

    [TestClass]
    public class QueryGenerationSubClass : AbstractComposer
    {

        static QueryGenerationSubClass()
        {
            ConfigLoader.Load("mysql");
        }


        [TestMethod]
        public void SelectStarSub()
        {
            var select = From<Order>().Select(order => order);
            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0");
            Console.WriteLine(select.Sql);
        }


        [TestMethod]
        public void SelectSomeSub()
        {
            var select = From<Order>().Select(order => new
            {
                order.OrderId,
                order.CustomerId
            });

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.ORDER_ID T0_ORDER_ID,T0.CUSTOMER_ID T0_CUSTOMER_ID from `ORDER` T0");
            Console.WriteLine(select.Sql);
        }

        [TestMethod]
        public void SelectWhereSub()
        {
            var select = From<Order>()
                .Where(order => order.CustomerId == 10)
                .Select(order => order);

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0 where T0.CUSTOMER_ID=10");
        }

        [TestMethod]
        public void SelectWithLimitSub()
        {
            var select = From<Order>()
                .Where(order => order.CustomerId == 10)
                .Select(order => order)
                .Limit(5);

            var sql = select.Sql;
            Assert.IsTrue(sql == "select T0.*  from `ORDER` T0 where T0.CUSTOMER_ID=10  limit 5");
        }

        [TestMethod]
        public void JoinInner2TablesSub()
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
        public void JoinInner2TablesWhereSub()
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
