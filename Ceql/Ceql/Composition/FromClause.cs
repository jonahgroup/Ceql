namespace Ceql.Composition
{
    using System;
    using System.Reflection; //needed for .net core
    using System.Collections.Generic;
    using System.Linq;
    using Ceql.Statements;
    using Ceql.Utils;
    using Attributes = Ceql.Contracts.Attributes;
    using Ceql.Contracts.Exceptions;

    public class FromClause
    {

        public EJoinType JoinType;

        public List<ExpressionAggregator> JoinExpression;
        public FromClause Parent;
        public SelectStatement SubSelect;

        public FromClause(SelectStatement selectClause = null)
        {
            JoinExpression = new List<ExpressionAggregator>();
            SubSelect = selectClause;
        }

        /// <summary>
        /// Returns from clause table name, in case of a nested join return the last table
        /// </summary>
        public string TableName
        {
            get
            {
                var sourceType = this.GetType().GetGenericArguments().Last();
                var tableAttr = TypeHelper.GetAttribute<Attributes.Table>(sourceType);

                if (tableAttr == null)
                {
                    throw new TableAttributeNotFoundException();
                }

                return tableAttr.Name;
            }
        }

        public string SchemaName
        {
            get
            {
                var sourceType = this.GetType().GetGenericArguments().Last();
                var schemaAttr = TypeHelper.GetAttribute<Attributes.Schema>(sourceType);

                if (schemaAttr == null)
                {
                    return null;
                }
                return schemaAttr.Name;
            }
        }

        public Type TableType
        {
            get
            {
                if (GetType().GetGenericArguments().Any())
                    return GetType().GetGenericArguments().Last();
                return null;
            }
        }

        public void AddJoinExpression(ExpressionAggregator expressionAggregator)
        {
            JoinExpression.Add(expressionAggregator);
        }

        public Dictionary<string, Type> GetTableList()
        {
            var d = new Dictionary<string, Type>();
            var parent = this;
            while (parent != null)
            {
                d.Add(parent.TableName,parent.TableType);
                parent = parent.Parent;
            }
            return d;
        }

    }

}
