using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        static int register = 0;
        static int initializeTorRegister = 0;

        static Client TorClient;

        static void Main(string[] args)
        {
            InitializeTor();

            WebClient client = new WebClient();
            client.Proxy = new WebProxy("127.0.0.1:8182");
            String Text = client.DownloadString("https://baconipsum.com/api/?type=meat-and-filler");
        }

        /// <summary>
        /// App.config içerisindeki verileri alarak tor.exe'yi 127.0.0.1:8182 adresinden başlatır.
        /// </summary>
        static void InitializeTor()
        {
            try
            {
                Process[] previous = Process.GetProcessesByName("tor");

                if (previous != null && previous.Length > 0)
                {
                    foreach (Process process in previous)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception)
                        {

                        }
                    }

                }

                ClientCreateParams createParameters = new ClientCreateParams();
                createParameters.ConfigurationFile = System.Configuration.ConfigurationManager.AppSettings["torConfigurationFile"];
                createParameters.ControlPassword = System.Configuration.ConfigurationManager.AppSettings["torControlPassword"];
                createParameters.ControlPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["torControlPort"]);
                createParameters.DefaultConfigurationFile = System.Configuration.ConfigurationManager.AppSettings["torDefaultConfigurationFile"];


                createParameters.Path = System.Configuration.ConfigurationManager.AppSettings["torPath"];


                createParameters.SetConfig(ConfigurationNames.AvoidDiskWrites, true);
                createParameters.SetConfig(ConfigurationNames.GeoIPFile, Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip"));
                createParameters.SetConfig(ConfigurationNames.GeoIPv6File, Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip6"));
                TorClient = Client.Create(createParameters);
                TorClient.Status.GetAllRouters();
                initializeTorRegister++;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Tor ağını kapatıp tekrar açarak bağlı bulunduğu nodu değiştirir. Aktif tor nodu artık kulllanılamıyorsa bu fonksiyonun çalıştırılması gerekir.
        /// </summary>
        static void RefreshTor()
        {
            TorClient.Stop(true);
            InitializeTor();
        }
    }
}

