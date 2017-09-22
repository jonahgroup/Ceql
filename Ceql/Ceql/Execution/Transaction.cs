namespace Ceql.Execution
{
    using Ceql.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Ceql.Composition;
    using Ceql.Configuration;

    public class Transaction : ITransaction
    {
        private Action<ITransaction> _body;
        private IDbConnection _connection;
        private IDataConnector _connector;

        public Transaction(Action<ITransaction> transactionBody)
        {
            _body = transactionBody;
        }

        public IEnumerable<T> Insert<T>(IEnumerable<T> entities) where T : ITable
        {
            return Insert(entities, new InsertClause<T>().Model);
        }
        
        public IEnumerable<T> FullInsert<T>(IEnumerable<T> entities) where T : ITable
        {
            return Insert(entities,new InsertClause<T>().FullModel);
        }

        private IEnumerable<T> Insert<T>(IEnumerable<T> entities, Model.InsertStatementModel<T> model) where T : ITable
        {
            var command = _connection.CreateCommand();

            foreach (var entity in entities)
            {
                command.CommandText = model.ApplyParameters(entity);
                _connector.PreInsert<T>(command, entity);
                command.ExecuteScalar();
                _connector.PostInsert<T>(command, entity);
            }
            return entities;
        }

        /// <summary>
        /// Generates and executes DELETE statement for each 
        /// entity in the entities collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void Delete<T>(IEnumerable<T> entities) where T : ITable
        {
            var model = new DeleteClause<T>().Model;
            var command = _connection.CreateCommand();

            foreach (var entity in entities)
            {
                command.CommandText = model.ApplyParameters(entity);
                command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Update statement is implemented as a series of DELETE and INSERT statements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void Update<T>(IEnumerable<T> entities) where T : ITable
        {
            Delete(entities);
            FullInsert(entities);
            return; 
        }

        /// <summary>
        /// Executes transaction statements
        /// </summary>
        public void Execute()
        {
            _connection = CeqlConfiguration.Instance.GetConnection();
            _connector = CeqlConfiguration.Instance.GetConnector();

            _connection.Open();

            var dbTransaction = _connection.BeginTransaction();

            try
            {
                _body(this);
                dbTransaction.Commit();
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
