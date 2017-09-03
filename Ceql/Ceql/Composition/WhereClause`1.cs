namespace Ceql.Composition
{
    using Ceql.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// Where clause for a single table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhereClause<T> : WhereClause
    {
        public WhereClause(FromClause<T> fromClause, Expression<BooleanExpression<T>> filter)
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

        public SelectClause<T, TResult> Select<TResult>(Expression<SelectExpression<T, TResult>> select)
        {
            return new SelectClause<T, TResult>((FromClause<T>)this.FromClause, this, select);
        }
    }
}
