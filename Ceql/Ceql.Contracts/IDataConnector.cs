using Ceql.Contracts.Configuration;
using System;
using System.Data;

namespace Ceql.Contracts
{
    public interface IDataConnector
    {
        IDbConnection GetDbConnection(IConnectionConfig config);
        void PreInsert<T>(IDbCommand connection, T table) where T : ITable;
        void PostInsert<T>(IDbCommand connection, T table) where T : ITable;
    }
}
