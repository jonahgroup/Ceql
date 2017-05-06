namespace Ceql.Tests.Bench
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ceql.Connectors.MySql;
    using Ceql.Tests.Bench.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class QueryExecution : AbstractComposer
    {

        static QueryExecution()
        {
            ConfigLoader.Load("mysql");
        }

        [TestMethod]
        public void SelectStarExec()
        {
            var select = From<Order>()
                .Select(order => order);

            var sql = select.Sql;
            var orders = select.ToList();
            Assert.IsTrue(orders != null);
        }

        [TestMethod]
        public void NullablePrimitives()
        {
            var customers = From<Customer>()
                .Select(customer => customer).ToList();

            Assert.IsTrue(customers.Any());
        }


        [TestMethod]
        public void InsertExec() {

            var orders = new List<Order>()
            {
                new Order {
                    CreateTs = DateTime.Now,
                    CustomerId = 10,
                    UserId = 546
                },
                new Order {
                    CreateTs = DateTime.Now,
                    CustomerId = 11,
                    UserId = 23
                },
                new Order {
                    CreateTs = DateTime.Now,
                    CustomerId = 11,
                    UserId = null
                }
            };

            Transaction(t =>
            {
                t.Insert(orders);
            }).Execute();
        }


        [TestMethod]
        public void SchemaDomainGeneration()
        {
            new DomainClassGenerator().GenerateDomain("ceql","Sample.Domain");
        }


    }
}
