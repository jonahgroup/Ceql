namespace Ceql.Composition
{
    using Ceql.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// Where clause for two source tables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class WhereClause<T, T1> : WhereClause
    {

        public WhereClause(FromClause<T, T1> fromClause, Expression<BooleanExpression<T, T1>> filter)
        {
            this.FromClause = fromClause;
            this.FilterExpression.Add(
                 new ExpressionAggregator()
                 {
                     Expression = filter,
                     ExpressionBoundClauses = CeqlUtils.GetStatementList(fromClause)
                 }
             );
        }

        public SelectClause<T, T1, TResult> Select<TResult>(Expression<SelectExpression<T, T1, TResult>> select)
        {
            return new SelectClause<T, T1, TResult>((FromClause<T, T1>)this.FromClause, this, select);
        }
    }
}
