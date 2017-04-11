using Ceql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ceql.Tests.Bench
{
    public static class ConfigLoader
    {

        public static void Load(string name) {
            CeqlConfiguration.Load(String.Format("Configs\\Ceql.{0}.json",name));
        }


    }
}
