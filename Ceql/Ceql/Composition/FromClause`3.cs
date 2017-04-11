using System.Linq.Expressions;
using Ceql.Utils;
using Ceql.Contracts;

namespace Ceql.Composition
{
    public class FromClause<T1, T2, T3> : FromClause
    {
        public FromClause(FromClause parent, Expression<BooleanExpression<T1, T2, T3>> join)
        {
            this.Parent = parent;
            JoinExpression.Add(new ExpressionAggregator()
            {
                Expression = join,
                ExpressionBoundClauses = CeqlUtils.GetStatementList(this)
            });

        }

        public FromClause(FromClause parent, SelectClause<T3> select, Expression<BooleanExpression<T1, T2, T3>> join) : this(parent, join)
        {
            this.SubSelect = select;
        }

        public WhereClause<T1, T2, T3> Where(Expression<BooleanExpression<T1, T2, T3>> filter)
        {
            return new WhereClause<T1, T2, T3>(this, filter);
        }

        public SelectClause<T1, T2, T3, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, T3, TResult>> select)
        {
            return new SelectClause<T1, T2, T3, TResult>(this, null, select);
        }

        //join to create 4 table from clause
        public FromClause<T1, T2, T3, T4> Join<T4>(Expression<BooleanExpression<T1, T2, T3, T4>> join) where T4 : ITable
        {
            return new FromClause<T1, T2, T3, T4>(this, join);
        }

        public FromClause<T1, T2, T3, T4> Left<T4>(Expression<BooleanExpression<T1, T2, T3, T4>> join) where T4 : ITable
        {
            var fc = new FromClause<T1, T2, T3, T4>(this, join);
            fc.JoinType = EJoinType.Left;
            return fc;
        }
    }

}
