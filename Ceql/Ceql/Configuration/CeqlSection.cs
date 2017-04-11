using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ceql.Configuration
{
  
    public class CeqlSection : ConfigurationSection
    {
        /*
        [ConfigurationProperty("connection")]
        public List<ConnectionConfig> Connections { get; set; }
        */
        [ConfigurationProperty("connections")]
        [ConfigurationCollection(typeof(ConnectionConfig), AddItemName = "connection")]
        public ConnectionCollection Connections {
            get { return (ConnectionCollection)this["connections"]; }
            set { this["connections"] = value; }
        }
    }


    public class ConnectionCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionConfig) element).Name;
        }
    }


    public class ConnectionConfig : ConfigurationElement
    {

        [ConfigurationProperty("name")]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("driver")]
        public string Driver
        {
            get { return (string) this["driver"]; }
            set { this["driver"] = value; }
        }
    }
}
