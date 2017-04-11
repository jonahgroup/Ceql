namespace Ceql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Ceql.Execution;
    using Ceql.Contracts;
    using Ceql.Contracts.Attributes;
    using Ceql.Composition;
    using Ceql.Statements;

    public abstract class AbstractComposer
    {

        public static ITransaction Transaction(Action<ITransaction> transactionBody)
        {

            return new Transaction(transactionBody);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string TableName(Type tableType)
        {
            var attr = tableType.GetTypeInfo().CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof (Table));

            if (attr == null) return null;
            return attr.ConstructorArguments[0].Value.ToString();
        }

        /// <summary>
        /// Returns a select clause that will result in select SQl however does not use any tables 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SelectClause<T> PseudoSelect<T>(Expression<SelectExpression<T>> expression)
        {
            return new SelectClause<T>(expression);
        }


        /// <summary>
        /// Top level table select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static FromClause<T> From<T>() where T : ITable
        {
            return new FromClause<T>();
        }

        /// <summary>
        /// Top level sub-query select
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectClause"></param>
        /// <returns></returns>
        public static FromClause<TResult> From<TResult>(SelectClause<TResult> selectClause)
        {
            return new FromClause<TResult>(selectClause);
        }


        /// <summary>
        /// In clause generator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool In<T>(IEnumerable<T> list, T item)
        {
            return true;
        }

        /// <summary>
        /// Like clause generator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="like"></param>
        /// <returns></returns>
        public static bool Like<T>(T item, string like)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T Max<T>(T item)
        {
            return item;
        }

        public static int Count<T>(T item)
        {
            return 0;
        }
    }
}
