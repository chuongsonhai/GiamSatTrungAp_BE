using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EVN.CMISApi
{
    public class ConnectConfig
    {
        public static ConnectConfig Instance
        {
            get
            {
                return Nested.ConnectConfig;
            }
        }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private ConnectConfig()
        {
            connectstring = ConfigurationManager.ConnectionStrings["cmisConnection"].ConnectionString; ;
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly ConnectConfig ConnectConfig =
                new ConnectConfig();
        }

        public string connectstring { get; set; }
    }

    public class SyncData
    {
        public string code { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public JArray data { get; set; }
    }
}