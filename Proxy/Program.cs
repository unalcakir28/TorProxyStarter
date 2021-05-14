using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Tor;
using Tor.Config;

namespace Proxy
{
    public static class Program
    {
        static Client client;

        static void Main(string[] args)
        {
            InitializeTor();

            WebClient webClient = new WebClient();
            webClient.Proxy = new WebProxy(client.Proxy.Address);
            Console.WriteLine(webClient.DownloadString("https://api.ipify.org/"));
            client.Dispose();
        }

        static void InitializeTor()
        {
            try
            {
                Process[] previous = Process.GetProcessesByName("tor");

                if (previous != null && previous.Length > 0)
                {
                    foreach (Process process in previous)
                        process.Kill();
                }

                ClientCreateParams createParameters = new ClientCreateParams();
                createParameters.ConfigurationFile = ConfigurationManager.AppSettings["torConfigurationFile"];
                createParameters.ControlPassword = ConfigurationManager.AppSettings["torControlPassword"];
                createParameters.ControlPort = Convert.ToInt32(ConfigurationManager.AppSettings["torControlPort"]);
                createParameters.DefaultConfigurationFile = ConfigurationManager.AppSettings["torDefaultConfigurationFile"];
                createParameters.Path = ConfigurationManager.AppSettings["torPath"];

                createParameters.SetConfig(ConfigurationNames.AvoidDiskWrites, true);
                createParameters.SetConfig(ConfigurationNames.GeoIPFile, Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip"));
                createParameters.SetConfig(ConfigurationNames.GeoIPv6File, Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip6"));

                client = Client.Create(createParameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

