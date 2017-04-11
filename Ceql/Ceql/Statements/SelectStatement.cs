namespace Ceql.Statements
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Ceql.Model;
    using Ceql.Utils;
    using Ceql.Composition;
    using Ceql.Contracts;
    using Ceql.Generation;


    public abstract class SelectStatement: ISqlExpression
    {
        private FromClause _fromClause;
        
        //subsset of the from clauses
        public List<FromClause> ExpressionBoundClauses;
        public FromClause FromClause {
            get { return _fromClause; }
            protected set
            {
                ExpressionBoundClauses = CeqlUtils.GetStatementList(value);
                _fromClause = value;
            }
        }

        public WhereClause WhereClause { get; protected set; }

        public Expression SelectExpression;
        public Type ResultType { get; set; }

        /// <summary>
        /// Indicates whether selection is distict
        /// </summary>
        public bool IsDistinct { get; set; }
        
        /// <summary>
        /// Sets the limit on results returned by the query
        /// </summary>
        public int ResultLimit { get; set; }
        
        /// <summary>
        /// Is set to to true on queries that may be cached
        /// without worrying about data consistency
        /// </summary>
        public bool IsCacheable { get; set; }

        public virtual string Sql
        {
            get { return SelectStatementGenerator.Instance.GetModel(this).Sql.Trim(); }
        }

        public virtual SelectStatementModel Model
        {
            get
            {
                return SelectStatementGenerator.Instance.GetModel(this);
            }
        }

        public void AddRootFromClause(FromClause fromClause)
        {
            var fC = FromClause;
            while (fC.Parent != null)
            {
                fC = fC.Parent;
            }

            fC.Parent = fromClause;
        }

        public void Clone(SelectStatement target)
        {
            target.FromClause = FromClause;
            target.WhereClause = WhereClause;
            target.SelectExpression = SelectExpression;

            target.ResultType = ResultType;
            target.IsDistinct = IsDistinct;
            target.ResultLimit = ResultLimit;
        }
    }



}
