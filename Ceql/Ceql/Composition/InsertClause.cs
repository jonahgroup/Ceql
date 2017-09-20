namespace Ceql.Composition
{
    using Ceql.Contracts;
    using Ceql.Statements;

    public class InsertClause<T> : InsertStatement<T> where T: ITable
    {
    }
}
