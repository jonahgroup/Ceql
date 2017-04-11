using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ceql.Composition;
using Ceql.Model;
using System.Reflection;
using Ceql.Contracts;
using Ceql.Formatters;

namespace Ceql.Expressions
{
    public class ConditionExpressionAnalyzer
    {

        private MemberAccessDecoder _memberAccessDecoder;
        private IConnectorFormatter _formatter;

        public ConditionExpressionAnalyzer(IConnectorFormatter formatter)
        {
            _formatter = formatter;
        }

        public object Sql(List<FromAlias> aliasList, Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if(lambda == null) throw new Exception("Invalid join expression");
            _memberAccessDecoder = new MemberAccessDecoder(lambda.Parameters.ToList(), aliasList);

            return Visit(lambda.Body).ToString();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object Visit(MemberExpression exp)
        {
            //var p = exp.Expression as ParameterExpression;
            var selectItem = _memberAccessDecoder.Decode(exp);

            if (selectItem == null)
            {
                var call = Expression.Lambda(exp).Compile();
                return call.DynamicInvoke(null);
            }
            return selectItem;
        }


        private List<string> ConcatenateLists(IEnumerable source)
        {
            
            var count = 0;
            var separator = ""; var list = "";
            var result = new List<string>();
            foreach (var item in source)
            {
                list += (separator + this._formatter.Format(item));
                separator = ",";
                count++;

                if (count == 1000)
                {
                    result.Add(list);
                    separator = list = "";
                    count = 0;
                }
            }
            if (count > 0)
            {
                result.Add(list);
            }
            return result;
        }


        private object Visit(MethodCallExpression exp)
        {
            var method = exp.Method;

            if (method.Name == "In" && method.DeclaringType == typeof (AbstractComposer))
            {
                var list = (IEnumerable)Visit(exp.Arguments[0]);
                var field = Visit(exp.Arguments[1]);

                var lists = ConcatenateLists(list);
                if (lists.Count > 1)
                {
                    return "(" + String.Join("AND", lists.Select(l=> field.ToString() + "  IN (" + l + ")") ) + ")";
                }
                return field.ToString() + " IN (" + lists[0] + ")";
            }

            if (method.Name == "Like" && method.DeclaringType == typeof (AbstractComposer))
            {
                return "LIKE ";
            }

            var instance = exp.Object!= null ? Visit(exp.Object) : null;
            if (instance is SelectAlias)
            {
                return _formatter.Format((ISelectAlias)instance, method);
            }
            return method.Invoke(instance, exp.Arguments.Select(Visit).ToArray());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private object Add(object left, object right)
        {
            if (left.GetType().GetTypeInfo().IsPrimitive && right.GetType().GetTypeInfo().IsPrimitive)
            {

                var call = Expression.Lambda(
                    Expression.Add(Expression.Constant(left), Expression.Constant(right))).Compile();
                var result = call.DynamicInvoke(null);
                return result;
            }
            return left + " + " + right;
        }

        /// <summary>
        /// Process binary expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object Visit(BinaryExpression exp)
        {
            var op = "";
            switch (exp.NodeType)
            {
                case ExpressionType.Equal:
                    op = "=";
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                case ExpressionType.NotEqual:
                    op = "<>";
                    break;

            }

            var left = Visit(exp.Left);
            var right = Visit(exp.Right);

            if (left is SelectAlias)
            {
                if (exp.NodeType == ExpressionType.NotEqual && right == null)
                {
                    return left + " is not null";
                }

                if (exp.NodeType == ExpressionType.Equal && right == null)
                {
                    return left + " is null";
                }


                return left + op + _formatter.Format((ISelectAlias)left,right);
            }

            return left + op + right;
        }



        private object Visit(NewExpression exp)
        {
            var call = Expression.Lambda(exp).Compile();
            var result = call.DynamicInvoke(null);
            return result;
        }



        private object CastExpression(UnaryExpression exp)
        {
            return Visit(exp.Operand);
        }


        /// <summary>
        /// Walks through the expressiont ree
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private object Visit(Expression exp)
        {

            var nodeType = exp.NodeType;

            switch (nodeType)
            {
                case ExpressionType.Add:
                    var add = exp as BinaryExpression;
                    return Add(Visit(add.Left), Visit(add.Right));
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.AndAlso:
                    var andAlso = exp as BinaryExpression;
                    return "(" + Visit(andAlso.Left) + " and " + Visit(andAlso.Right) + ")";
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Call:
                    var call = exp as MethodCallExpression;
                    return Visit(call);
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    var con = exp as ConstantExpression;
                    return con.Value;
                case ExpressionType.Convert:
                    return CastExpression(exp as UnaryExpression);
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.Equal:
                    return Visit(exp as BinaryExpression);
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.GreaterThan:
                    return Visit(exp as BinaryExpression);
                case ExpressionType.GreaterThanOrEqual:
                    return Visit(exp as BinaryExpression);

                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    return Visit((exp as LambdaExpression).Body);
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.LessThan:
                    return Visit(exp as BinaryExpression);
                  
                case ExpressionType.LessThanOrEqual:
                    return Visit(exp as BinaryExpression);

                case ExpressionType.ListInit:
                    break;
                case ExpressionType.MemberAccess:
                    return Visit(exp as MemberExpression);
                case ExpressionType.MemberInit:
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
                    return Visit(exp as NewExpression);
                    
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.NotEqual:
                    return Visit(exp as BinaryExpression);
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrElse:
                    var orElse = exp as BinaryExpression;
                    return "(" + Visit(orElse.Left) + " or " + Visit(orElse.Right) + ")";
                case ExpressionType.Parameter:
                    Visit(exp as ParameterExpression);
                    break;
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
            return "";
        }


    }


}
