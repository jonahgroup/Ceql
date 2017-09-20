namespace Ceql.Generation
{
    using Ceql.Statements;
    using Ceql.Contracts;
    using Ceql.Model;
    using Ceql.Utils;
    using Ceql.Configuration;
    using Attributes = Ceql.Contracts.Attributes;
    using System.Linq;
    using System.Reflection;

    public class DeleteStatementGenerator
    {
        public static DeleteStatementModel<T> Generate<T>(DeleteStatement<T> statement) where T : ITable
        {
            var table = TypeHelper.GetType<Attributes.Table>(statement.Type);

            // get primary-key fields only
            var fields = TypeHelper.GetPropertiesForAttribute<Attributes.Field>(table)
                .Where(f => f.GetCustomAttribute<Attributes.PrimaryKey>() != null);

            var tableName = TypeHelper.GetAttribute<Attributes.Table>(table).Name;
            var schemaAtr = TypeHelper.GetAttribute<Attributes.Schema>(table);

            var schemaName = schemaAtr != null ? schemaAtr.Name : null;

            return new DeleteStatementModel<T>(CeqlConfiguration.Instance.GetConnectorFormatter())
            {
                Fields = fields,
                TableName = tableName,
                SchemaName = schemaName
            };
        }
    }
}
