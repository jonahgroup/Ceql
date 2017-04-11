namespace Ceql.Composition
{
    using Ceql.Contracts;
    using Ceql.Statements;
    using System;

    public class InsertClause<T> : InsertStatement<T> where T: ITable
    {
        public InsertClause()
        {
            Type = typeof(T);
        }
    }

}
