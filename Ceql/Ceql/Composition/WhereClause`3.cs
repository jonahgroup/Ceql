namespace Ceql.Composition
{
    using Ceql.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// Where clause for three source tables
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class WhereClause<T1, T2, T3> : WhereClause
    {

        public WhereClause(FromClause<T1, T2, T3> fromClause, Expression<BooleanExpression<T1, T2, T3>> filter)
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

        public SelectClause<T1, T2, T3, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, TResult>((FromClause<T1, T2, T3>)this.FromClause, this, select);
        }
    }
}
