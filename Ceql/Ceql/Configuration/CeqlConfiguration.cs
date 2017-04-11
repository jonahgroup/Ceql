using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ceql.Configuration;
using System.Data;
using Ceql.Contracts;

namespace Ceql.Configuration
{
    public class CeqlConfiguration
    {

        private static CeqlConfiguration _instance = null;
        //static constructor
        static CeqlConfiguration()
        {
            var settings = ConfigurationManager.GetSection("Ceql") as CeqlSection;
            if (settings == null) return;
            if (_instance == null) _instance = new CeqlConfiguration(settings);
        }

        //singleton getter
        public static CeqlConfiguration Instance {
            get { return _instance; }
        } 


        /// <summary>
        /// Use to load configuration file
        /// </summary>
        /// <param name="configFilePath"></param>
        public static void Load(string configFilePath)
        {
            var fileMap = new ConfigurationFileMap(configFilePath);
            var conf = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
            var settings = conf.GetSection("Ceql") as CeqlSection;
            _instance = new CeqlConfiguration(settings);
        }

    
        private readonly List<ConnectionConfig> _connections;

        /// <summary>
        /// Private constructor, configuration object may not be created manually
        /// </summary>
        /// <param name="section"></param>
        private CeqlConfiguration(CeqlSection section)
        {
            _connections = new List<ConnectionConfig>();
            foreach (ConnectionConfig connection in section.Connections)
            {
                _connections.Add(connection);
            }
        }

        /// <summary>
        /// Returns a list of configured connections
        /// </summary>
        public List<ConnectionConfig> Connections
        {
            get { return _connections; }
        }

        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public IConnectorFormatter GetConnectorFormatter()
        {
            throw new NotImplementedException();
        }

        public IDataConnector GetConnector()
        {
            throw new NotImplementedException();
        }

    }
}
