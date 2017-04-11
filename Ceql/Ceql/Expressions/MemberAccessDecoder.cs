using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ceql.Model;
using System.Reflection;
using Ceql.Contracts.Attributes;

namespace Ceql.Expressions
{
    public class MemberAccessDecoder
    {
        private readonly List<ParameterExpression> _parameters;
        private readonly List<FromAlias> _aliasList;

        public MemberAccessDecoder(List<ParameterExpression> parameters, List<FromAlias> aliasList)
        {
            this._parameters = parameters;
            this._aliasList = aliasList;
        }


        public SelectAlias Decode(MemberExpression exp)
        {
            var p = exp.Expression as ParameterExpression;
            if (p == null) return null;

            var pIdx = _parameters.IndexOf(p);
            var alias = _aliasList[pIdx];

            string fieldName = null;
            SelectAlias parent = null;

            //start checking if alias from clause is a sub select
            if (alias.SubGeneratedSelect != null)
            {
                parent = alias.SubGeneratedSelect.SelectList.FirstOrDefault(s => s.TargetMember == exp.Member);

                if (alias.SubGeneratedSelect.IsAllSelect)
                {
                    parent = alias.SubGeneratedSelect.SelectList[0];
                }

                if (parent == null) return null;

                if (parent is SelectAllAlias)
                {
                    fieldName = (parent as SelectAllAlias).GetFieldName(exp.Member.Name);
                }
                else
                {
                    fieldName = parent.Alias;
                }
            }
            else
            {

                var property = alias.TableType.GetProperties().FirstOrDefault(prop => prop.Name == exp.Member.Name);
                if (property == null) throw new Exception("Unable to find member " + exp.Member.Name + "in type " + alias.TableType);
                
                var fieldAttr = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(Field));
                if (fieldAttr == null) throw new Exception("Unable to find field attribute " + exp.Member.Name + "in type " + alias.TableType); ;

                if (fieldAttr.ConstructorArguments.Count > 0)
                    fieldName = fieldAttr.ConstructorArguments[0].Value.ToString();
                else
                    fieldName = exp.Member.Name;
            }

            return new SelectAlias()
            {
                Name = fieldName,
                Source = alias.Name,
                Alias = alias.Name + "_" + fieldName,
                Parent = parent,
                SourceType = alias.TableType,
                SourceMember = exp.Member
            };
        }


        public SelectAlias Decode(ParameterExpression exp)
        {

            var pIdx = _parameters.IndexOf(exp);
            var alias = _aliasList[pIdx];

            return new SelectAllAlias()
            {
                Name = "*",
                Source = alias.Name,
                Alias = "",
                SourceType = alias.TableType
            };
        }


    }
}
