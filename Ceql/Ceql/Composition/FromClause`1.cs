using System.Linq.Expressions;
using Ceql.Contracts;


namespace Ceql.Composition
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FromClause<T> : FromClause
    {

        /// <summary>
        /// Table select query
        /// </summary>
        public FromClause()
        {
        }

        /// <summary>
        /// Subselect from clause
        /// </summary>
        /// <param name="select"></param>
        public FromClause(SelectClause<T> select)
        {
            this.SubSelect = select;
        }

        //inner table join
        public FromClause<T, T1> Join<T1>(Expression<BooleanExpression<T, T1>> join) where T1 : ITable
        {
            return new FromClause<T, T1>(this, join);
        }

        //left table join
        public FromClause<T, TResult> Left<TResult>(Expression<BooleanExpression<T, TResult>> join)
        {
            var from = new FromClause<T, TResult>(this, join);
            from.JoinType = EJoinType.Left;
            return from;
        }

        //inner sub select join
        public FromClause<T, T1> Join<T1>(SelectClause<T1> select, Expression<BooleanExpression<T, T1>> join)
        {
            return new FromClause<T, T1>(this, select, join);
        }



        public WhereClause<T> Where(Expression<BooleanExpression<T>> filter)
        {
            return new WhereClause<T>(this, filter);
        }

        public SelectClause<T, TResult> Select<TResult>(Expression<SelectExpression<T, TResult>> select)
        {
            return new SelectClause<T, TResult>(this, null, select);
        }
    }

}
