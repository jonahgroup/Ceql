namespace Ceql.Connectors
{
    using System;
    using System.Reflection;
    using System.Data;
    using Ceql.Contracts;
    using global::MySql.Data.MySqlClient;
    using Ceql.Contracts.Configuration;
    using Ceql.Utils;
    using Attributes = Ceql.Contracts.Attributes;
    using System.Linq;

    public class MySqlDbConnector : IDataConnector
    {
        public IDbConnection GetDbConnection(IConnectionConfig config)
        {
            var connection = new MySqlConnection()
            {
                ConnectionString = config.ConnectionString
            };

            return connection;
        }

        public void PreInsert<T>(IDbCommand command, T entity) where T : ITable
        {
            return;
        }


        public void PostInsert<T>(IDbCommand command, T entity) where T : ITable
        {

            var type = TypeHelper.GetType<Attributes.Table>(entity.GetType());
            var autoSequence = TypeHelper
                .GetPropertiesForAttribute<Attributes.AutoSequence>(type)
                .FirstOrDefault();

            // no autosequence fields, nothing to do here
            if (autoSequence == null)
            {
                return;
            }

            
            command.CommandText = "SELECT LAST_INSERT_ID()";
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read()) return;

                var value = Convert.ChangeType(reader[0],autoSequence.PropertyType);
                autoSequence.SetValue(entity, value);
            }

        }

    }
}
