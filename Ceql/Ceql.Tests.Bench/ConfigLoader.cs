using Ceql.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Tests.Bench
{
    public static class ConfigLoader
    {
        public static void Load(string name)
        {
            CeqlConfiguration.Load(String.Format("Configs\\Ceql.{0}.config", name));
        }
    }
}
