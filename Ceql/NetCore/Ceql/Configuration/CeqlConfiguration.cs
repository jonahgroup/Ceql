namespace Ceql.Configuration
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Data;
    using Ceql.Contracts.Configuration;
    using Ceql.Contracts.Exceptions;
    using Ceql.Utils;
    using Ceql.Contracts;

    public class CeqlConfiguration : ICeqlConfiguration
    {

        private static ICeqlConfiguration _instance = null;
       
        //static constructor
        static CeqlConfiguration()
        {
            try
            {
                var section = JObject.Parse(File.ReadAllText("appsettings.json"))
                    .GetValue("Ceql")
                    .ToObject<CeqlSection>();

                _instance = new CeqlConfiguration(section);
            }
            catch (Exception) {
                return;        
            }
        }

        //singleton getter
        public static ICeqlConfiguration Instance
        {
            get { return _instance; }
        }


        /// <summary>
        /// Use to load configuration file
        /// </summary>
        /// <param name="configFilePath"></param>
        public static void Load(string configFilePath)
        {
            var section = JObject.Parse(File.ReadAllText(configFilePath))
                .GetValue("Ceql")
                .ToObject<CeqlSection>();

            _instance = new CeqlConfiguration(section);
        }


        private readonly ICeqlSection _section;

        /// <summary>
        /// Private constructor, configuration object may not be created manually
        /// </summary>
        /// <param name="section"></param>
        private CeqlConfiguration(ICeqlSection section)
        {
            _section = section;
        }

        /// <summary>
        /// Returns a list of configured connections
        /// </summary>
        public List<IConnectionConfig> Connections
        {
            get { return _section.GetConnections().ToList(); }
        }

        /// <summary>
        /// Gets default connection
        /// </summary>
        public IDbConnection GetConnection()
        {
            var connectionConfig = SelectConnectionConfig();
            var connector = TypeHelper.LoadType<IDataConnector>(connectionConfig.Connector);

            if (connector == null)
            {
                throw new ConnectorInstanceException("Unable to create connector instance for type " + connectionConfig.Connector);
            }

            return connector.GetDbConnection(connectionConfig);
        }

        public IConnectorFormatter GetConnectorFormatter()
        {
            var connectionConfig = SelectConnectionConfig();
            var formatter = TypeHelper.LoadType<IConnectorFormatter>(connectionConfig.Formatter);
            return formatter;
        }

        // todo (dr): convert connector to a singleton
        public IDataConnector GetConnector()
        {
            var connectionConfig = SelectConnectionConfig();
            var connector = TypeHelper.LoadType<IDataConnector>(connectionConfig.Connector);
            return connector;
        }

        private IConnectionConfig SelectConnectionConfig()
        {
            // use default connection config
            var connectionConfig = Connections.Where(c => c.Default).FirstOrDefault();

            // use first connection if nothing is marked default
            connectionConfig = connectionConfig ?? Connections.FirstOrDefault();

            if (connectionConfig == null)
            {
                throw new InvalidConfigurationException("Unable to locate connection configuration");
            }

            return connectionConfig;
        }
    }
}
