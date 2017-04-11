using System.Collections.Generic;
using System.Linq.Expressions;
using Ceql.Expressions;
using Ceql.Utils;


namespace Ceql.Composition
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereClause
    {
        public List<ExpressionAggregator> FilterExpression = new List<ExpressionAggregator>();
        public FromClause FromClause;
    }
    
    
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

        public SelectClause<T,TResult> Select<TResult>(Expression<SelectExpression<T, TResult>> select)
        {
            return new SelectClause<T,TResult>((FromClause<T>)this.FromClause, this, select);
        }
    }


    /// <summary>
    /// Where clause for two source tables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class WhereClause<T,T1> : WhereClause 
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

        public SelectClause<T, T1, TResult> Select<TResult>(Expression<SelectExpression<T,T1,TResult>> select)
        {
            return new SelectClause<T, T1, TResult>((FromClause<T, T1>)this.FromClause, this, select);
        }

    }


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
