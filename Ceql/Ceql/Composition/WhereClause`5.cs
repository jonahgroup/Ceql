namespace Ceql.Composition
{
    using Ceql.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class WhereClause<T1, T2, T3, T4, T5> : WhereClause
    {

        public WhereClause(FromClause<T1, T2, T3, T4, T5> fromClause, Expression<BooleanExpression<T1, T2, T3, T4, T5>> filter)
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

        public SelectClause<T1, T2, T3, T4, T5, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, T4, T5, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, T4, T5, TResult>((FromClause<T1, T2, T3, T4, T5>)this.FromClause, this, select);
        }

    }
}
