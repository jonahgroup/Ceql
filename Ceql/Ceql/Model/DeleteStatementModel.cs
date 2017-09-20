using Ceql.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Model
{
    public class DeleteStatementModel<T> : StatementModel<T> where T : ITable
    {
        public DeleteStatementModel(IConnectorFormatter formatter) :base(formatter)
        {}

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
                _sql = String.Format("DELETE FROM {0} WHERE {1}",
                    Formatter.TableNameEscape(SchemaName, TableName),
                    String.Join(" AND ", Fields.Select(f => {
                        return f.GetCustomAttribute<Contracts.Attributes.Field>().Name + " = @p" + count++;
                    })));

                return _sql;
            }
        }
    }
}
