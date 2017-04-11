namespace Ceql.Composition
{
    using System.Linq.Expressions;

    public class SelectClause<T, T1, TResult> : SelectClause<TResult>
    {

        public SelectClause(FromClause<T, T1> from, WhereClause<T, T1> where, Expression<SelectExpression<T, T1, TResult>> select)
        {
            FromClause = from;
            WhereClause = where;
            SelectExpression = select;
            ResultType = typeof(TResult);
        }
    }
}
