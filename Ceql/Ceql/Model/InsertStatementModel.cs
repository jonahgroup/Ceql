using Ceql.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Model
{
    public class InsertStatementModel<T> : StatementModel<T> where T : ITable
    {
        public InsertStatementModel(IConnectorFormatter formatter) : base(formatter)
        { }
        
        private object lck = new object();
        private string _sql;

        protected override string GetSql()
        {
            if (_sql != null)
            {
                return _sql;
            }

            lock (lck)
            {
                if (_sql != null)
                {
                    return _sql;
                }

                var count = 0;
                _sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                    Formatter.TableNameEscape(SchemaName, TableName),
                    String.Join(",", Fields.Select(f => Formatter.ColumnNameEscape(f.GetCustomAttribute<Contracts.Attributes.Field>().Name))),
                    String.Join(",", Fields.Select(f => "@p" + count++)));

                return _sql;
            }
        }
    }
}
