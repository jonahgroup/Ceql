namespace Ceql.Composition
{

    using System.Linq.Expressions;

    public class SelectClause<T1, T2, T3, T4, TResult> : SelectClause<TResult>
    {

        public SelectClause(FromClause<T1, T2, T3, T4> from, WhereClause<T1, T2, T3, T4> where, Expression<SelectExpression<T1, T2, T3, T4, TResult>> select)
        {
            this.FromClause = from;
            this.WhereClause = where;
            this.SelectExpression = select;
            this.ResultType = typeof(TResult);
        }
    }
}
