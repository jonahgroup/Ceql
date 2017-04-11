using System.Linq.Expressions;
using Ceql.Utils;
using Ceql.Contracts;

namespace Ceql.Composition
{
    public class FromClause<T1, T2, T3, T4> : FromClause
    {
        public FromClause(FromClause parent, Expression<BooleanExpression<T1, T2, T3, T4>> join)
        {
            this.Parent = parent;
            JoinExpression.Add(new ExpressionAggregator()
            {
                Expression = join,
                ExpressionBoundClauses = CeqlUtils.GetStatementList(this)
            });
        }

        //join to create 4 table from clause
        public FromClause<T1, T2, T3, T4, T5> Join<T5>(Expression<BooleanExpression<T1, T2, T3, T4, T5>> join) where T5 : ITable
        {
            return new FromClause<T1, T2, T3, T4, T5>(this, join);
        }

        //left table join
        public FromClause<T1, T2, T3, T4, T5> Left<T5>(Expression<BooleanExpression<T1, T2, T3, T4, T5>> join) where T5 : ITable
        {
            var fc = new FromClause<T1, T2, T3, T4, T5>(this, join);
            fc.JoinType = EJoinType.Left;
            return fc;
        }


        public SelectClause<T1, T2, T3, T4, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, T4, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, T4, TResult>(this, null, select);
        }

    }
}
