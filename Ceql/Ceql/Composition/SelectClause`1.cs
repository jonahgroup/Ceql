namespace Ceql.Composition
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Ceql.Execution;
    using Ceql.Statements;

    public class SelectClause<T> : SelectStatement, IEnumerable<T>
    {

        public SelectClause()
        {
        }

        public SelectClause(Expression<SelectExpression<T>> select)
        {
            this.SelectExpression = select;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new SelectEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
