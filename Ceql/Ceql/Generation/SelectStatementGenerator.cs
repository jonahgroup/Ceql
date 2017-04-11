namespace Ceql.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ceql.Composition;
    using Ceql.Expressions;
    using Ceql.Statements;
    using Ceql.Model;
    using Ceql.Formatters;
    using Ceql.Contracts;
    using Ceql.Configuration;

    public class SelectStatementGenerator
    {
        
        public static SelectStatementGenerator Instance
        {
            get { return new SelectStatementGenerator(CeqlConfiguration.Instance.GetConnectorFormatter()); }
        }

        IConnectorFormatter _formatter;

        /// <summary>
        /// Private constructor
        /// </summary>
        protected SelectStatementGenerator(IConnectorFormatter formatter)
        {
            _formatter = formatter;
        }

        /// <summary>
        /// Returns SQL statement for the provided SelectClause
        /// </summary>
        /// <param name="selectClause"></param>
        /// <returns></returns>
        public SelectStatementModel GetModel(SelectStatement selectClause)
        {
            
            
            var select = selectClause.IsDistinct ? "select distinct " : "select ";
            var from = "";
            var where = "";
            var limit = "";

            //this is a pseudo select
            if (selectClause.FromClause == null)
            {

                var _list = BuildSelectList(selectClause, null).ToList();
                
                return new SelectStatementModel()
                {
                    Sql = select + SelectSql(_list),
                    SelectList = _list
                };
            }

            //generate alias list
            var aliasList = GetAliasList(selectClause.FromClause);

            //process from clause
            if (selectClause.FromClause != null) from = FromSql(_formatter, selectClause, aliasList);
            if (selectClause.WhereClause != null) where = WhereSql(selectClause, aliasList);

            var selectList =  BuildSelectList(selectClause, aliasList).ToList();
            
            select = select + SelectSql(selectList);

            var groupby = "";
            if (selectList.Any(a => a.IsGroupRequired == true)) {
                groupby = GroupBySql(selectList.Where(s=>s.IsGroupRequired == false));
            }

            //set limit on result set
            if (selectClause.ResultLimit > 0) limit = "limit " + selectClause.ResultLimit;

            return new SelectStatementModel()
            {
                Sql = select + " " + from + " " + where + " " + groupby + " " +limit,
                AliasList = aliasList,
                SelectList = selectList.ToList(),
                IsAllSelect = selectList.Aggregate(true,(a,b)=>b.IsAllSelect & a)
            };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        private string SelectSql(IEnumerable<SelectAlias> list )
        {
            return String.Join(",", list.Select(x => x.ToString() + " " + x.Alias));
        }

        private string GroupBySql(IEnumerable<SelectAlias> list) {
            if (!list.Any()) return "";
            return " group by " + String.Join(",", list.Select(x => x.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectClause"></param>
        /// <returns></returns>
        private string WhereSql(SelectStatement selectClause, List<FromAlias> aliasList )
        {
            var whereClause = selectClause.WhereClause;
            var analyzer = new ConditionExpressionAnalyzer(_formatter);

            var whereSql = "where ";

            for (var i = 0; i < whereClause.FilterExpression.Count; i++)
            {
                var filter = whereClause.FilterExpression[i];
                if (i >= 1) //dont care about operator if it is a first join expression
                    switch (filter.Operator)
                    {
                        case EBooleanOperator.And:
                            whereSql += " and ";
                            break;
                        case EBooleanOperator.Or:
                            whereSql += " or ";
                            break;
                        case null:
                            break;
                        default:
                            break;
                    }

                whereSql += analyzer.Sql(FilterExpressionAliasList(aliasList,filter.ExpressionBoundClauses), filter.Expression);    
            }

            return whereSql;
        }

        /// <summary>
        /// Returns SQL statement for the FromClause member of the SelectClause
        /// </summary>
        /// <param name="selectClause"></param>
        /// <returns></returns>
        private string FromSql(IConnectorFormatter formatter, SelectStatement selectClause, List<FromAlias> list )
        {
            var fromClause = selectClause.FromClause;
            var sql = "from " + BuildJoins(formatter, fromClause, list);
            return sql;
        }


        /// <summary>
        /// Returns collection of the select statement alias
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        private IEnumerable<SelectAlias> BuildSelectList(SelectStatement select, List<FromAlias> aliasList )
        {
            var analyzer = new SelectExpressionAnalyzer(_formatter, select, FilterExpressionAliasList(aliasList, select.ExpressionBoundClauses));
            return (List<SelectAlias>)analyzer.Sql();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="aliasList"></param>
        /// <returns></returns>
        protected string BuildJoins(IConnectorFormatter formatter, FromClause from, List<FromAlias> aliasList )
        {
            var alias = aliasList.First(x => x.FromClause == from);

            var joinType = " join ";
            switch (from.JoinType)
            {
                case EJoinType.Inner:
                    joinType = " join ";
                    break;
                case EJoinType.Left:
                    joinType = " left join ";
                    break;
                case EJoinType.Right:
                    joinType = " right join ";
                    break;
                default:
                    joinType = " join ";
                    break;
            }
            
            //joined tables
            if (from.Parent != null)
            {
                var analyzer = new ConditionExpressionAnalyzer(_formatter);
                var statement = BuildJoins(formatter, from.Parent, aliasList) + joinType + TableSql(formatter, from, aliasList) + " on ";

                var joinCondition = "";
               for(var i=0; i<from.JoinExpression.Count; i++)
                {
                    var j = from.JoinExpression[i];
                    var faList = FilterExpressionAliasList(aliasList, j.ExpressionBoundClauses);
                    
                    if(i >= 1) //dont care about operator if it is a first join expression
                    switch (j.Operator)
                    {
                        case EBooleanOperator.And:
                            joinCondition += " and ";
                            break;
                        case EBooleanOperator.Or:
                            joinCondition += " or ";
                            break;
                        case null:
                            break;
                        default:
                            break;
                    }

                    joinCondition += analyzer.Sql(faList, j.Expression);    
                }
                
                //append join condition
                statement += joinCondition;


                return statement;
            }
            
            //driver tables
            return TableSql(formatter, from, aliasList);
        }

        /// <summary>
        /// Filters master alias list to get an ordered alias list only for the ExpressionBoundClauses references in the select or join expression
        /// </summary>
        /// <param name="aliasList"></param>
        /// <param name="clauses">From clauses in the order of the statement, first from clause comes first</param>
        /// <returns></returns>
        private List<FromAlias> FilterExpressionAliasList(List<FromAlias> aliasList, List<FromClause> clauses)
        {
            if (clauses == null || clauses.Count == 0) return aliasList;

            var faList = new List<FromAlias>();
            for (var i = 0; i < clauses.Count; i++)
            {
                var als = aliasList.FirstOrDefault(a => a.FromClause == clauses[i]);
                faList.Add(als);
            }
            return faList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private List<FromAlias> GetAliasList(FromClause from)
        {

            var joinCount = 0;
            var node = from;
            while (node.Parent != null)
            {
                node = node.Parent;
                ++joinCount;
            }
        
            var list = new List<FromAlias>();

            node = from;
            while (node != null)
            {
                var alias = "T" + (joinCount - list.Count);

                var fa = new FromAlias()
                {
                    FromClause = node,
                    TableType = node.TableType,
                    Name = alias
                };

                if (node.SubSelect != null)
                {
                    fa.SubGeneratedSelect = GetModel(node.SubSelect);
                }
                
                list.Add(fa);
                
                node = node.Parent;
            }

            list.Reverse();
            return list;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private string TableSql(IConnectorFormatter formatter, FromClause from, List<FromAlias> aliases )
        {
            //get alias for this clause
            var alias = aliases.First(x => x.FromClause == from);

            if (from.SubSelect != null)
            {
                return "(" + alias.SubGeneratedSelect.Sql + ") " + alias.Name;
            }

            return formatter.TableNameEscape(from.SchemaName,from.TableName) + " " + alias.Name;
        }
    }

}
