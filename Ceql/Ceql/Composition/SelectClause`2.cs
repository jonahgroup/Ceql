namespace Ceql.Composition
{
    using System.Linq.Expressions;

    public class SelectClause<T, TResult> : SelectClause<TResult>
    {

        public SelectClause(FromClause<T> from, WhereClause<T> where, Expression<SelectExpression<T, TResult>> select)
        {
            this.FromClause = from;
            this.WhereClause = where;
            this.SelectExpression = select;
            this.ResultType = typeof(TResult);
        }
    }
}
