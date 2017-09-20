using Ceql.Contracts;
using Ceql.Model;
using Ceql.Statements;

namespace Ceql.Composition
{
    public class DeleteClause<T> : DeleteStatement<T> where T: ITable
    {
    }
}
