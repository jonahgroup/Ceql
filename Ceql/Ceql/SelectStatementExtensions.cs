namespace Ceql
{
    using Ceql.Composition;
    using Ceql.Contracts;
    using Ceql.Contracts.Attributes;
    using Ceql.Statements;
    using System.Linq;
    using System.Reflection;

    public static class SelectStatementExtensions
    {

        /// <summary>
        /// Instructs query composer to apply distinct selection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static T Distinct<T>(this T select) where T : SelectStatement
        {
            select.IsDistinct = true;
            return select;
        }

        /// <summary>
        /// Limits result set to a fixed number of records
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static T Limit<T>(this T select, int limit) where T : SelectStatement
        {
            select.ResultLimit = limit;
            return select;
        }

        /// <summary>
        /// Return record count for the current select this will not produce result set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectClause<int> Count<T>(this SelectClause<T> select)
        {
            return new FromClause<T>(select).Select(x => AbstractComposer.Count(x));
        }



        public static object ColummValue(this ITable table, string colName)
        {
            //var m = System.Reflection.MethodBase.GetCurrentMethod();
            var typeArg = table.GetType();
            //find property with a column name
            foreach (var prop in typeArg.GetProperties())
            {

                var attr = prop.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(Field));
                if (attr == null) continue;
                if (attr.ConstructorArguments[0].Value.ToString() == colName)
                {
                    return prop.GetValue(table);
                }
            }
            return null;
        }

    }
}
