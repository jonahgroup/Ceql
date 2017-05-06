using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Ceql.Composition;
using Ceql.Expressions;
using Ceql.Model;
using Ceql.Contracts.Attributes;

namespace Ceql.Execution
{

    internal class ParameterResultInfo
    {
        
        public Type Type;
        public List<string> ArgumentMapping;
        public List<Tuple<string, string, MemberInfo>> MemberMapping;
        public ConstructorInfo Constructor;
        public object[] ConstArgumentsBuffer;

        public object CreateInstance(IVirtualDataReader reader)
        {
            
            //set contructor arguments
            for (var i = 0; i < ConstArgumentsBuffer.Length; i++)
            {
                var idx = ArgumentMapping[i];
                ConstArgumentsBuffer[i] = reader[idx];
            }

            //create instance
            var instance = Constructor.Invoke(ConstArgumentsBuffer);

            //set properties on an instance
            for (var i = 0; i < MemberMapping.Count; i++)
            {
                var tuple = MemberMapping[i];
                var v = reader[tuple.Item1];

                // null values are skipped
                // property vlues will be set to their defaults
                if (v == DBNull.Value) continue;

                //set field
                var field = tuple.Item3 as FieldInfo;
                if(field != null) SetValue(instance,field,v);

                //set property only if set method is available
                var property = tuple.Item3 as PropertyInfo;
                if(property != null && property.SetMethod != null) SetValue(instance, property, v);

            }

            return instance;
        }


        /// <summary>
        /// Sets value on a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="info"></param>
        /// <param name="value"></param>
        private void SetValue(object instance, PropertyInfo info, object value)
        {
            if (IsNullable(info))
            {
                info.SetValue(instance, value);
                return;
            }
            info.SetValue(instance,value);
        }

        /// <summary>
        /// Sets value on a field info
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="info"></param>
        /// <param name="value"></param>
        private void SetValue(object instance, FieldInfo info, object value)
        {
            if (IsNullable(info))
            {
                info.SetValue(instance, value);
                return;
            }
            info.SetValue(instance,value);
        }



        public Boolean IsNullable(PropertyInfo pInfo)
        {
            return pInfo.PropertyType.GetTypeInfo().IsGenericType &&
                   pInfo.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public Boolean IsNullable(FieldInfo fInfo)
        {
            return fInfo.FieldType.GetTypeInfo().IsGenericType &&
                   fInfo.FieldType.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

    }


    class SelectEnumerator<TResult> : IEnumerator<TResult>
    {

        
        private IEnumerable<SelectAlias> _selectList; 

        private List<TResult> _result;
        private IEnumerator<TResult> listEnumerator;
        public SelectEnumerator(SelectClause<TResult> selectClause )
        {
            var rowLambda = selectClause.SelectExpression as LambdaExpression;
            var compiledRowLambda = rowLambda.Compile();
            var paramInfo = new List<ParameterResultInfo>();

            var generatedSelect = selectClause.Model;
            _selectList = generatedSelect.SelectList;
            
            foreach (var x in rowLambda.Parameters)
            {
                //look for the type in the select list
                var sel = _selectList.FirstOrDefault(s => s.SourceType == x.Type);
                if (sel == null)
                {
                    paramInfo.Add(null);
                    continue;
                }

                paramInfo.Add(new ParameterResultInfo()
                {
                    Type = x.Type,
                    ArgumentMapping = ConstructorArgumentsMap(x.Type),
                    MemberMapping = PropertyMapping(x.Type),
                    Constructor = x.Type.GetConstructors()[0],
                    ConstArgumentsBuffer = new object[x.Type.GetConstructors()[0].GetParameters().Length]
                });

            }

            var connection = VirtualDataSource.GetConnection();
            
            _result = new List<TResult>();

            connection.Run(selectClause, (reader) =>
            {
                /*
                    for each row:
                    for each parameter
                    1. instantiate parameter
                    2. set property values
                    3. execute lambda expression
                    4. cast to result
                */

                var arguments = new object[paramInfo.Count];

                for (var i = 0; i < paramInfo.Count; i++)
                {
                    var p = paramInfo[i];
                    if (p != null) arguments[i] = p.CreateInstance(reader);
                }
                _result.Add((TResult) compiledRowLambda.DynamicInvoke(arguments));
            });

            listEnumerator = _result.GetEnumerator();
        }

        public void Dispose()
        {
            listEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return listEnumerator.MoveNext();
        }

        public void Reset()
        {
            listEnumerator.Reset();
        }
        public TResult Current {
            get { return listEnumerator.Current; }
        }
        object IEnumerator.Current
        {
            get { return listEnumerator.Current; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<string> ConstructorArgumentsMap(Type type)
        {
            var constructor = type.GetConstructors()[0];
            var arguments = constructor.GetParameters();

            var argumentMap = new List<string>();
            var selects = _selectList.Where(l => l.SourceType == type).ToList();

            foreach (var p in arguments)
            {
                var tsel = selects.FirstOrDefault(s => s.SourceMember.Name == p.Name);
                if (tsel != null)
                {
                    argumentMap.Add(tsel.Alias);
                }
                else
                {
                    argumentMap.Add(null);
                }

            }

            return argumentMap;
        }


        private List<Tuple<string, string, MemberInfo>> PropertyMapping(Type type)
        {
            var result = new List<Tuple<string, string,MemberInfo>>();

            var properties = type.GetProperties();
            var fields = type.GetFields();

            var selects = _selectList.Where(l => l.SourceType == type).ToList();

            //with only a single select with name * index all class properties
            if (selects.Any(x=>x.Name == "*"))
            {

                foreach (var prop in properties)
                {
                    var tField = prop.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof (Field));
                    if (tField == null) continue;
                    result.Add(new Tuple<string, string,MemberInfo>(tField.ConstructorArguments[0].Value.ToString(), prop.Name, prop));
                }
                return result;
            }


            foreach (var prop in properties)
            {
                var tsel = selects.FirstOrDefault(s => s.SourceMember.Name == prop.Name);
                if (tsel == null) continue;
                result.Add(new Tuple<string, string, MemberInfo>(tsel.Alias, prop.Name,prop));
            }

            
            foreach (var field in fields)
            {
                var tsel = selects.FirstOrDefault(s => s.SourceMember.Name == field.Name);
                if (tsel == null) continue;
                result.Add(new Tuple<string, string,MemberInfo>(tsel.Alias, field.Name,field));
            }
            

            return result;
        }





    }
}
