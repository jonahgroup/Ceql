using System.Linq.Expressions;
using Ceql.Utils;
using Ceql.Contracts;

namespace Ceql.Composition
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class FromClause<T1, T2> : FromClause
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="join"></param>
        public FromClause(FromClause parent, Expression<BooleanExpression<T1, T2>> join)
            : base()
        {
            this.Parent = parent;
            JoinExpression.Add(new ExpressionAggregator()
            {
                Expression = join,
                ExpressionBoundClauses = CeqlUtils.GetStatementList(this)
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="select"></param>
        /// <param name="join"></param>
        public FromClause(FromClause parent, SelectClause<T2> select, Expression<BooleanExpression<T1, T2>> join)
            : this(parent, join)
        {
            this.SubSelect = select;
        }


        public FromClause<T1, T2, T3> Join<T3>(Expression<BooleanExpression<T1, T2, T3>> join) where T3 : ITable
        {
            return new FromClause<T1, T2, T3>(this, join);
        }

        public FromClause<T1, T2, T3> Join<T3>(SelectClause<T3> select, Expression<BooleanExpression<T1, T2, T3>> join)
        {
            return new FromClause<T1, T2, T3>(this, select, join);
        }

        public FromClause<T1, T2, T3> Left<T3>(Expression<BooleanExpression<T1, T2, T3>> join) where T3 : ITable
        {
            var fc = new FromClause<T1, T2, T3>(this, join);
            fc.JoinType = EJoinType.Left;
            return fc;
        }

        public WhereClause<T1, T2> Where(Expression<BooleanExpression<T1, T2>> filter)
        {
            return new WhereClause<T1, T2>(this, filter);
        }

        public SelectClause<T1, T2, TResult> Select<TResult>(Expression<SelectExpression<T1, T2, TResult>> select)
        {
            return new SelectClause<T1, T2, TResult>(this, null, select);
        }

    }

}
