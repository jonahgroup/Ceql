using System;
using System.Collections.Generic;
using Ceql.Utils;
using Ceql.Statements;

namespace Ceql.Execution.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class DataSourceCache
    {

        //get log instance
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DataSourceCache));
        
        private static readonly object _lck = new object();

        //singleton harness
        private static DataSourceCache _instance; 
        public static DataSourceCache Instance {
            get
            {
                if (_instance != null) return _instance;
                lock (_lck)
                {
                    if (_instance == null) _instance = new DataSourceCache();
                }
                return _instance;
            }
        }

        //cache entry lock
        private static object _cacheLck = new object();

        //keeps collection of the cache-state objects to track cache status
        private readonly Dictionary<string, DataSourceCacheEntry> _cacheMap = new Dictionary<string, DataSourceCacheEntry>();


        public volatile ECacheState State; //{ get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataSourceCache()
        {
            State = ECacheState.Busy;
        }

        /// <summary>
        /// Performs cache entry lookup for cacheable queries only
        /// </summary>
        /// <param name="select"></param>
        /// <returns>IVirtualDataReader object</returns>
        public IVirtualDataReader GetReader(SelectStatement select)
        {

            //process reader
            if (!select.IsCacheable) return new VirtualDbReader(select);
            
            //find cache entry
            var key = CeqlUtils.GetCacheKey(select.Sql);

            //no key found
            if (!_cacheMap.ContainsKey(key))
            lock (_cacheLck)
            {
                if (!_cacheMap.ContainsKey(key))
                {
                    var newEntry = new DataSourceCacheEntry(){Key = key};
                    _cacheMap.Add(key, newEntry);
                    return new VirtualDbCacheableReader(newEntry, select);
                }
            }

            //return cached data reader
            var entry = _cacheMap[key];
            if (entry.State == ECacheState.Ready)
            {
                return entry.Store.DataReader;
            }

            //return default reader 
            return new VirtualDbReader(select);

        }
    }


    public enum ECacheState
    {
        New, Ready, Busy, Invalid
    }


    /// <summary>
    /// Must be a thread-safe cache state tracker object
    /// </summary>
    public class DataSourceCacheEntry
    {
        public string Key { get; set; }
        private ECacheState _state;
        public ECacheState State
        {
            get {
                lock (this)
                {
                    return _state;
                }
            }
            set
            {
                lock (this)
                {
                    _state = value;
                }
            }
        }

        private ICacheStore _store;
        public ICacheStore Store
        {
            get
            {
                lock (this)
                {
                    return _store;
                }
            }
            set {
                lock (this)
                {
                    _store = value;
                }
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICacheStore
    {
        IVirtualDataReader DataReader { get; }
    }


    [Serializable]
    public class InMemoryCacheStore :  ICacheStore
    {
        public IVirtualDataReader DataReader {
            get { return new InMemoryCacheStoreReader(this);}
        }

        public List<string> Fields { get; set; }
        public List<object[]> Data { get; set; }

        public InMemoryCacheStore()
        {
            Data = new List<object[]>();
        }
    }

  
    public class InMemoryCacheStoreReader : IVirtualDataReader
    {

        private int _position=-1;
        private InMemoryCacheStore _store;
        
        private Dictionary<string,int> _fieldIndex = new Dictionary<string, int>(); 
        public InMemoryCacheStoreReader(InMemoryCacheStore store)
        {
            _store = store;
            for (var i = 0; i < store.Fields.Count; i++)
            {
                _fieldIndex.Add(store.Fields[i],i);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        object IVirtualDataReader.this[int i]
        {
            get { return _store.Data[_position][i]; }
        }

        object IVirtualDataReader.this[string c]
        {
            get
            {
                var idx = _fieldIndex[c];
                return _store.Data[_position][idx];
            }
        }

        public bool Read()
        {
            _position++;
            if (_position >= _store.Data.Count) return false;
            return true;
        }

        public void Close()
        {
            //noop
        }
    }



}
