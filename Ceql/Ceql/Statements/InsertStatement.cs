using Ceql.Contracts;
using Ceql.Generation;
using Ceql.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ceql.Statements
{
    public class InsertStatement<T> : ISqlExpression where T : ITable
    {
        public Type Type { get; protected set; }

        public string Sql
        {
            get
            {
                return Model.Sql;
            }
        }

        public InsertStatementModel<T> Model
        {
            get
            {
                return InsertStatementGenerator.Generate<T>(this);
            }
        }

    }
}
