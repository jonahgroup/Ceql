namespace Ceql.Execution
{
    using Ceql.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ceql.Composition;
    using Ceql.Generation;
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

        public IEnumerable<T> Insert<T>(IEnumerable<T> values) where T : ITable
        {
            var model = new InsertClause<T>().Model;

            var command = _connection.CreateCommand();

            foreach (var value in values)
            {
                command.CommandText = model.ApplyParameters(value);
                _connector.PreInsert<T>(command, value);
                command.ExecuteScalar();
                _connector.PostInsert<T>(command, value);
            }

            return values;
        }

        public DeleteClause<T> Delete<T>(IEnumerable<T> values)
        {
            return new DeleteClause<T>();
        }

        public UpdateClause<T> Update<T>(IEnumerable<T> values)
        {
            return new UpdateClause<T>();
        }

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
