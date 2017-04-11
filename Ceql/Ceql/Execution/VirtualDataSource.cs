using System;
using System.Diagnostics;
using Ceql.Composition;


namespace Ceql.Execution
{
    
    public class VirtualDataSource : IDisposable
    {

        //get log
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(VirtualDataSource));


        [ThreadStatic]
        private static VirtualDataSource _dataSourceInstance;
        public static VirtualDataSource GetConnection(string key = null)
        {

            if (_dataSourceInstance == null) _dataSourceInstance = new VirtualDataSource(key);
            return _dataSourceInstance;
            
        }


        private VirtualDataSource(string key = null)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>

        public void Run<TResult>(SelectClause<TResult> select, Action<IVirtualDataReader> action)
        {
            if (action == null) return; //quit no one is listening

            //reader may be sourced thorugh data connection, in-memory cache, file based cache etc...
            var sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            var reader = ReaderLocator.GetReader(select);
            sWatch.Stop();
            Log.Debug("Query time: " + sWatch.ElapsedMilliseconds + " ms ");
            try
            {
                while (reader.Read())
                {
                    action(reader);
                }
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
           // Connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

           // Connection.Close();
            
        }
    }
}
