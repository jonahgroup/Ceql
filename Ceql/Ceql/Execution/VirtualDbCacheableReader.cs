using System;
using System.IO;
using System.Threading.Tasks;
using Ceql.Composition;
using Ceql.Execution.Cache;
using Ceql.Statements;

namespace Ceql.Execution
{
    public class VirtualDbCacheableReader : VirtualDbReader
    {

        //get log
        private static log4net.ILog log =
            log4net.LogManager.GetLogger(typeof(VirtualDbReader));

        private readonly DataSourceCacheEntry _entry;
        private readonly Task _spoolerTask;
        
        public VirtualDbCacheableReader(DataSourceCacheEntry entry, SelectStatement select)
            :base(select)
        {
            _entry = entry;
            
            _spoolerTask = new Task(() =>
            {
                var cacheReader = new VirtualDbReader(select);
                try
                {
                    _entry.State = ECacheState.Busy;
                    
                    var store = new InMemoryCacheStore
                    {
                        Fields = cacheReader.FieldNames()
                    };

                    
                    while (cacheReader.Read())
                    {
                        var row = new object[store.Fields.Count];
                        for (var j = 0; j < row.Length; j++)
                        {
                            row[j] = cacheReader[j];
                        }
                        store.Data.Add(row);
                    }

                    _entry.Store = store;
                    
                    //write to file
                    using(FileStream fs = new FileStream(entry.Key + ".cache", FileMode.Create)){
                        
                       // var formatter = new BinaryFormatter();
                       // formatter.Serialize(fs, store);
                    }
                    
                    _entry.State = ECacheState.Ready;
                }
                catch (Exception exception)
                {
                    _entry.State = ECacheState.Invalid;
                    log.Error(exception);
                    throw;
                }
                finally
                {
                    cacheReader.Close();
                }
            });

            _spoolerTask.Start();
        }
        
    }
}
