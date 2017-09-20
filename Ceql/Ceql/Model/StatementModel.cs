using Ceql.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Model
{
    public abstract class StatementModel<T> where T : ITable
    {
        public IEnumerable<PropertyInfo> Fields { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }

        protected IConnectorFormatter Formatter;

        public string Sql
        {
            get
            {
                return GetSql();
            }
        }

        public StatementModel(IConnectorFormatter formatter)
        {
            Formatter = formatter;
        }

        protected abstract string GetSql();

        public string ApplyParameters(T value)
        {
            var fields = Fields.ToArray();
            // todo (dr): inefficient, refactor
            var sql = Sql;
            for (var i = fields.Length - 1; i >= 0; i--)
            {
                var field = fields[i];
                var v = field.GetValue(value);
                sql = sql.Replace(("@p" + i), Formatter.Format(v).ToString());
            }
            return sql;
        }
    }
}
