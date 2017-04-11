using Ceql.Contracts;
using Ceql.Model;

namespace Ceql.Composition
{
    public class DeleteClause : ISqlExpression
    {
        public string Sql { get; private set; }
    }

    public class DeleteClause<T> : DeleteClause
    {
    }
}
