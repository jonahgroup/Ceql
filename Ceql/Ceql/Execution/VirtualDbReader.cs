using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ceql.Composition;
using Ceql.Configuration;
using Ceql.Statements;

namespace Ceql.Execution
{
    public class VirtualDbReader : IVirtualDataReader
    {

        //get Log
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(VirtualDbReader));
        
        private const int RetryCount = 5;
        protected  IDataReader Reader;
        protected  IDbConnection Connection;

        public VirtualDbReader(SelectStatement select)
        {
           OpenDbConnection(select);
        }

        public virtual object this[int i]
        {
            get { return Reader[i]; }
        }

        public virtual object this[string c]
        {
            get { return Reader[c]; }
        }

        public virtual bool Read()
        {
            return Reader.Read();
        }

        public List<string> FieldNames()
        {
            var list = new List<string>();
            for (var i = 0; i < Reader.FieldCount; i++)
            {
                list.Add(Reader.GetName(i));
            }
            return list;
        }


        public void Close()
        {
           // Reader.Close();
            Connection.Close();
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }



        protected void OpenDbConnection(SelectStatement select)
        {
            var config = CeqlConfiguration.Instance;

            var sql = select.Sql;

           
            IDbCommand command = null;

            Connection = config.GetConnection();
            var retry = RetryCount;
            while (retry-- > 0)
            {

                try
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.Open();
                    }

                    command = Connection.CreateCommand();
                    command.CommandText = sql;
                    break;
                }
                catch (Exception)
                {
                    if (retry <= 0) throw;
                    Log.Debug(Thread.CurrentThread.ManagedThreadId + "Retrying connection: "+ retry);
                    Thread.Sleep(500 * new Random().Next(1, 3));
                }
            }

            if (command == null)
            {
                throw new Exception("Failed to connect");
            }

            //reader retry
            retry = RetryCount;
            while (retry-- > 0)
            {
                try
                {
                    Reader = command.ExecuteReader();
                    break;
                }
                catch (Exception)
                {
                    if (retry <= 0) throw;
                    Log.Debug(Thread.CurrentThread.ManagedThreadId + "Retrying query: " + retry);
                    Thread.Sleep(500 * new Random().Next(1, 3));
                }
            }
        
        }

    }
}
