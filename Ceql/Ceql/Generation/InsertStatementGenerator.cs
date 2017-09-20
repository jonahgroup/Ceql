namespace Ceql.Generation
{
    using Ceql.Composition;
    using Ceql.Statements;
    using Ceql.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ceql.Model;
    using Attributes = Ceql.Contracts.Attributes;
    using Ceql.Utils;
    using Ceql.Configuration;
    using System.Reflection;

    public static class InsertStatementGenerator
    {
        public static InsertStatementModel<T> Generate<T>(InsertStatement<T> statement, bool isFull = false) where T : ITable
        {
            var table = TypeHelper.GetType<Attributes.Table>(statement.Type);

            // dont take  keys fields
            IEnumerable<PropertyInfo> fields = null;

            if (isFull)
            {
                fields = TypeHelper.GetPropertiesForAttribute<Attributes.Field>(table);
            }
            else
            {
                fields = TypeHelper.GetPropertiesForAttribute<Attributes.Field>(table)
                    .Where(f => f.GetCustomAttribute<Attributes.AutoSequence>() == null);
            }

            var tableName = TypeHelper.GetAttribute<Attributes.Table>(table).Name;
            var schemaAtr = TypeHelper.GetAttribute<Attributes.Schema>(table);

            var schemaName = schemaAtr != null ? schemaAtr.Name : null;

            return new InsertStatementModel<T>(CeqlConfiguration.Instance.GetConnectorFormatter())
            {
                Fields = fields,
                TableName = tableName,
                SchemaName = schemaName
            };
        }
    }
}
