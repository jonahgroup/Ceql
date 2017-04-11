using System.Linq.Expressions;
using Ceql.Utils;
using Ceql.Contracts;

namespace Ceql.Composition
{
    public class FromClause<T1, T2, T3, T4, T5> : FromClause
    {
        public FromClause(FromClause parent, Expression<BooleanExpression<T1, T2, T3, T4, T5>> join)
        {
            this.Parent = parent;
            JoinExpression.Add(new ExpressionAggregator()
            {
                Expression = join,
                ExpressionBoundClauses = CeqlUtils.GetStatementList(this)
            });
        }


        public SelectClause<T1, T2, T3, T4, T5, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, T4, T5, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, T4, T5, TResult>(this, null, select);
        }

    }
}
