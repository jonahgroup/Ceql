namespace Ceql.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Ceql.Model;
    using Ceql.Statements;
    using Ceql.Formatters;
    using Ceql.Contracts;

    class SelectExpressionAnalyzer
    {
        private readonly List<FromAlias> _aliasList;
        private SelectStatement _select;
        private IConnectorFormatter _formatter;

        public SelectExpressionAnalyzer(IConnectorFormatter formatter, SelectStatement select, List<FromAlias> aliasList )
        {
            this._aliasList = aliasList;
            this._select = select;
            _formatter = formatter;

            var lambda = select.SelectExpression as LambdaExpression;
            if (lambda == null) throw new Exception("Invalid expression");

            _parameters = lambda.Parameters;
            _expression = lambda;
            _memberAccessDecoder = new MemberAccessDecoder(_parameters.ToList(), _aliasList);
        }

        public object Sql()
        {
            return Extract();
        }

        private readonly ReadOnlyCollection<ParameterExpression> _parameters;
        private readonly LambdaExpression _expression;

        private List<SelectAlias> selectList = new List<SelectAlias>();
        private MemberAccessDecoder _memberAccessDecoder;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SelectAlias> Extract()
        {
            /*  this is a pseudo select 
                - select expression must be a parameterless lambda
                - expression body is new, member init, parameter
            */
            switch (_expression.Body.NodeType)
            {
                case ExpressionType.New:
                selectList.AddRange(VisitNewExpression(_expression.Body as NewExpression));
                    break;

                case ExpressionType.MemberInit:
                selectList.AddRange(VisitMemberInitExpression(_expression.Body as MemberInitExpression));
                    break;

                case ExpressionType.Parameter:
                selectList.AddRange(VisitParameterExpression(_expression.Body as ParameterExpression));
                    break;
                default:
                    selectList.AddRange(Visit(_expression.Body));
                break;
            }

            return selectList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> Visit(MemberExpression exp)
        {
            var list = new List<SelectAlias>();
            var item = _memberAccessDecoder.Decode(exp);
            if (item != null) list.Add(item);
            return  list;
        }

        /// <summary>
        /// Select * scenario
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> VisitParameterExpression(ParameterExpression exp)
        {
            var list = new List<SelectAlias>();
            var item = _memberAccessDecoder.Decode(exp);
            if (item != null) list.Add(item);

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> VisitBinaryExpression(BinaryExpression exp)
        {
            var sa = new SelectAlias();

            var left = Visit(exp.Left);
            var right = Visit(exp.Right);

            sa.Children.AddRange(left);
            sa.Children.AddRange(right);
            
            sa.Alias = "C0";

            selectList.Add(sa);
            return new List<SelectAlias>() { sa };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        private object VisitMemberBinding(MemberBinding binding) { 
            if(binding is MemberAssignment) return Visit((binding as MemberAssignment).Expression);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        private List<SelectAlias> Visit(MethodCallExpression call)
        {
            
            if (call.Method.Name == "Count")
            {
                var members = Visit(call.Arguments[0]);
                foreach (var member in members) {
                    member.Function = "count";
                    member.Alias = "count_" + member.Alias;
                }
                return members;
            }

            if (call.Method.Name == "Max")
            {
                var members = Visit(call.Arguments[0]);

                foreach (var member in members) {
                    member.Function = "max";
                    member.Alias = "max_" + member.Alias;
                    member.IsGroupRequired = true;
                }
                return members;
            }
            throw new Exception("Invalid select clause method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> VisitUnaryExpression(UnaryExpression exp)
        {
            return Visit(exp.Operand);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> VisitMemberInitExpression(MemberInitExpression exp)
        {
            var idx = 0;
            var list = new List<SelectAlias>();

            foreach (var p in exp.Bindings)
            {
                var item = VisitMemberBinding(p) as SelectAlias;
                if (item != null)
                {
                    item.TargetType = exp.Type;
                    //add to list
                    list.Add(item);
                }
                else
                {
                    var value = Expression.Lambda(((MemberAssignment)p).Expression).Compile().DynamicInvoke();
                    list.Add(new SelectAlias()
                    {
                        Alias = p.Member.Name,
                        TargetMember = p.Member,
                        TargetType = exp.Type,
                        Value = _formatter.Format(value)
                    });        
                }
                idx++;
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> VisitNewExpression(NewExpression exp)
        {
            
            var idx = 0;
            var list = new List<SelectAlias>();

            foreach (var a in exp.Arguments)
            {
                var items = Visit(a);
                if (items != null && items.Count > 0)
                {
                    foreach(var item in items) {
                        item.TargetType = exp.Type;
                        item.TargetMember = exp.Members[idx];
                        item.Expression = a;
                    }
                    //add to list
                    list.AddRange(items);
                }
                else
                {
                    var value = Expression.Lambda(a).Compile().DynamicInvoke();
                    list.Add(new SelectAlias()
                    {
                        Alias = exp.Members[idx].Name,
                        TargetMember = exp.Members[idx],
                        TargetType = exp.Type,
                        Value = _formatter.Format(value)
                    });
                }
                idx++;
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private List<SelectAlias> Visit(Expression exp)
        {
            var nodeType = exp.NodeType;

            switch (nodeType)
                {
                    case ExpressionType.Add:
                        return VisitBinaryExpression(exp as BinaryExpression);
                    case ExpressionType.AddChecked:
                        break;
                    case ExpressionType.And:
                        break;
                    case ExpressionType.AndAlso:
                        break;
                    case ExpressionType.ArrayLength:
                        break;
                    case ExpressionType.ArrayIndex:
                        break;
                    case ExpressionType.Call:
                        return Visit(exp as MethodCallExpression);
                    case ExpressionType.Coalesce:
                        break;
                    case ExpressionType.Conditional:
                        break;
                    case ExpressionType.Constant:
                        break;
                    case ExpressionType.Convert:
                        return Visit( (exp as UnaryExpression).Operand);
                    case ExpressionType.ConvertChecked:
                        break;
                    case ExpressionType.Divide:
                        break;
                    case ExpressionType.Equal:
                        break;
                    case ExpressionType.ExclusiveOr:
                        break;
                    case ExpressionType.GreaterThan:
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        break;
                    case ExpressionType.Invoke:
                        break;
                    case ExpressionType.Lambda:
                        return Visit((exp as LambdaExpression).Body);
                    case ExpressionType.LeftShift:
                        break;
                    case ExpressionType.LessThan:
                        break;
                    case ExpressionType.LessThanOrEqual:
                        break;
                    case ExpressionType.ListInit:
                        break;
                    case ExpressionType.MemberAccess:
                        return Visit(exp as MemberExpression);
                    case ExpressionType.MemberInit:
                        VisitMemberInitExpression(exp as MemberInitExpression);
                        break;
                    case ExpressionType.Modulo:
                        break;
                    case ExpressionType.Multiply:
                        break;
                    case ExpressionType.MultiplyChecked:
                        break;
                    case ExpressionType.Negate:
                        break;
                    case ExpressionType.UnaryPlus:
                        break;
                    case ExpressionType.NegateChecked:
                        break;
                    case ExpressionType.New:
                        VisitNewExpression(exp as NewExpression);
                        break;
                    case ExpressionType.NewArrayInit:
                        break;
                    case ExpressionType.NewArrayBounds:
                        break;
                    case ExpressionType.Not:
                        return VisitUnaryExpression(exp as UnaryExpression);
                    case ExpressionType.NotEqual:
                        break;
                    case ExpressionType.Or:
                        break;
                    case ExpressionType.OrElse:
                        break;
                    case ExpressionType.Parameter:
                        /*  
                         *  if parameter expression in the body means we 
                         *  are selecting ALL from the source table
                         */
                        return VisitParameterExpression(exp as ParameterExpression);
                    case ExpressionType.Power:
                        break;
                    case ExpressionType.Quote:
                        break;
                    case ExpressionType.RightShift:
                        break;
                    case ExpressionType.Subtract:
                        break;
                    case ExpressionType.SubtractChecked:
                        break;
                    case ExpressionType.TypeAs:
                        break;
                    case ExpressionType.TypeIs:
                        break;
                    case ExpressionType.Assign:
                        break;
                    case ExpressionType.Block:
                        break;
                    case ExpressionType.DebugInfo:
                        break;
                    case ExpressionType.Decrement:
                        break;
                    case ExpressionType.Dynamic:
                        break;
                    case ExpressionType.Default:
                        break;
                    case ExpressionType.Extension:
                        break;
                    case ExpressionType.Goto:
                        break;
                    case ExpressionType.Increment:
                        break;
                    case ExpressionType.Index:
                        break;
                    case ExpressionType.Label:
                        break;
                    case ExpressionType.RuntimeVariables:
                        break;
                    case ExpressionType.Loop:
                        break;
                    case ExpressionType.Switch:
                        break;
                    case ExpressionType.Throw:
                        break;
                    case ExpressionType.Try:
                        break;
                    case ExpressionType.Unbox:
                        break;
                    case ExpressionType.AddAssign:
                        break;
                    case ExpressionType.AndAssign:
                        break;
                    case ExpressionType.DivideAssign:
                        break;
                    case ExpressionType.ExclusiveOrAssign:
                        break;
                    case ExpressionType.LeftShiftAssign:
                        break;
                    case ExpressionType.ModuloAssign:
                        break;
                    case ExpressionType.MultiplyAssign:
                        break;
                    case ExpressionType.OrAssign:
                        break;
                    case ExpressionType.PowerAssign:
                        break;
                    case ExpressionType.RightShiftAssign:
                        break;
                    case ExpressionType.SubtractAssign:
                        break;
                    case ExpressionType.AddAssignChecked:
                        break;
                    case ExpressionType.MultiplyAssignChecked:
                        break;
                    case ExpressionType.SubtractAssignChecked:
                        break;
                    case ExpressionType.PreIncrementAssign:
                        break;
                    case ExpressionType.PreDecrementAssign:
                        break;
                    case ExpressionType.PostIncrementAssign:
                        break;
                    case ExpressionType.PostDecrementAssign:
                        break;
                    case ExpressionType.TypeEqual:
                        break;
                    case ExpressionType.OnesComplement:
                        break;
                    case ExpressionType.IsTrue:
                        break;
                    case ExpressionType.IsFalse:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            return null;
        }
    }
}
