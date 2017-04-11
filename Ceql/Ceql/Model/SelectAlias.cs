using Ceql.Contracts;
using Ceql.Contracts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectAlias : ISelectAlias
    {
        public string Name;
        public string Alias;

        public string Source;
        public Type SourceType;
        public MemberInfo SourceMember;
        public Type TargetType;
        public MemberInfo TargetMember;

        public bool IsAllSelect;

        public List<SelectAlias> Children = new List<SelectAlias>();

        public Expression Expression;

        public SelectAlias Parent;

        public string Function;

        public bool IsGroupRequired;

        public object Value;

        public string ToSqlString()
        {
            if (Function != null)
                return Function + "(" + Source + "." + Name + ") " + Alias;
            return Source + "." + Name + " " + Alias;
        }

        public override string ToString()
        {
            if (Value != null) return Value.ToString();
            if (Function != null)
                return Function + "(" + Source + "." + Name + ")";
            return Source + "." + Name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SelectAllAlias : SelectAlias
    {

        public SelectAllAlias() : base()
        {
            IsAllSelect = true;
        }

        public string GetFieldName(string propName)
        {
            var prop = SourceType.GetProperties().FirstOrDefault(p => p.Name == propName);
            if(prop == null) throw new Exception("Unable to find proprty name " + propName);

            var attr = prop.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof (Field));
            if(attr == null) throw new Exception("Field attribute is not found on property " + propName);

            return attr.ConstructorArguments[0].Value.ToString();
        }
    }

}
