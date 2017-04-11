using System;
using Ceql.Composition;
using Ceql.Execution.Cache;


namespace Ceql.Execution
{
    public class ReaderLocator
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReaderLocator));

        public static IVirtualDataReader GetReader<TResult>(SelectClause<TResult> select)
        {
            //data cache
            var cache = DataSourceCache.Instance;

            try
            {
                //locate reader
                return cache.GetReader(select);
            }
            //silent but logged - cache lookup fail
            catch (Exception e)
            {
                log.Error("Cache lookup fail");
                log.Error(e);
                return new VirtualDbReader(select);
            }

        }
    }
}
