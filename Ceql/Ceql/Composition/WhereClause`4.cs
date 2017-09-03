namespace Ceql.Composition
{
    using Ceql.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public class WhereClause<T1, T2, T3, T4> : WhereClause
    {

        public WhereClause(FromClause<T1, T2, T3, T4> fromClause, Expression<BooleanExpression<T1, T2, T3, T4>> filter)
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

        public SelectClause<T1, T2, T3, T4, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, T4, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, T4, TResult>((FromClause<T1, T2, T3, T4>)this.FromClause, this, select);
        }

    }
}
