namespace Ceql.Composition
{

    using System.Linq.Expressions;

    public class SelectClause<T1, T2, T3, TResult> : SelectClause<TResult>
    {

        public SelectClause(FromClause<T1, T2, T3> from, WhereClause<T1, T2, T3> where, Expression<SelectExpression<T1, T2, T3, TResult>> select)
        {
            this.FromClause = from;
            this.WhereClause = where;
            this.SelectExpression = select;
            this.ResultType = typeof(TResult);
        }
    }
}
