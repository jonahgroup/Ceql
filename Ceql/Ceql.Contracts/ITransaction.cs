namespace Ceql.Contracts
{
    using System.Collections.Generic;

    public interface ITransaction
    {
        void Execute();
        IEnumerable<T> Insert<T>(IEnumerable<T> records) where T : ITable;
        IEnumerable<T> FullInsert<T>(IEnumerable<T> records) where T : ITable;
        void Delete<T>(IEnumerable<T> records) where T : ITable;
        void Update<T>(IEnumerable<T> records) where T : ITable;
    }
}
