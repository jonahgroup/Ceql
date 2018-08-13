using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Contracts
{
    public interface IConnectorFormatter
    {
        object Format(ISelectAlias instance, object source);

        object Format(object obj);

        string TableNameEscape(string schemaName, string tableName);

        string ColumnNameEscape(string columnName);
    }
}
