namespace Ceql.Formatters
{
    using System;
    using System.Reflection;
    using Ceql.Model;
    using Ceql.Contracts;

    public abstract class BaseFormatter : IConnectorFormatter
    {
        public virtual object Format(ISelectAlias instance, object source)
        {
            if (source is int)          return source.ToString();
            if (source is long)         return source.ToString();
            if (source is double)       return source.ToString();
            if (source is SelectAlias)  return source.ToString();
            if (source is SqlSnippet)   return source.ToString();
            
            if (source is DateTime)
            {

               var result =  String.Format("TIMEZONE(TO_TIMESTAMP('{0:yyyy-MM-dd hhh:mm:ss}', 'YYYY-MM-DD HH24:MI:SS'),'America/Toronto','UTC')",source);
               return result;

            }
            if (source is MethodInfo)
            {
                return FormatMethodInfo(instance, (MethodInfo)source);
            }

            return "'" + source.ToString() + "'";
        }

        public virtual object Format(object obj)
        {
            if (obj is DateTime)
            {

                var result = String.Format("TIMEZONE(TO_TIMESTAMP('{0:yyyy-MM-dd hhh:mm:ss}', 'YYYY-MM-DD HH24:MI:SS'),'America/Toronto','UTC')", obj);
                return result;
            }
            return "'" + obj.ToString() + "'";        
        }

        public virtual object FormatMethodInfo(ISelectAlias instance, MethodInfo mi)
        {
            if (mi.Name == "ToString") return new SqlSnippet("CAST( "+instance.ToString() + " as VARCHAR(10))");
            throw new Exception("Unsupported method");
        }

        public object FormatSelectAlias(SelectAlias alias)
        {
            return alias.ToSqlString();
        }

        /// <summary>
        /// Does not escape table names
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract string TableNameEscape(string schemaName, string tableName);
    }
}
