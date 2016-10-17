using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RouteTable
{
    public static class Config
    {
        static Config()
        {
            Directory.CreateDirectory(Output);
        }
        public static string Location {
            get
            {
                return Properties.Settings.Default.CurrentLocation;
            }
        }

        public static string Platform
        {
            get
            {
                return Properties.Settings.Default.CurrentPlatform;
            }
        }

        public static string Output
        {
            get
            {
                return Properties.Settings.Default.Outputs;
            }
        }
        public static class Remote
        {
            static Remote()
            {
                Directory.CreateDirectory(Properties.Settings.Default.RouteTables);              
            }
            public static string Path
            {
                get
                {
                    return System.IO.Path.Combine(Properties.Settings.Default.RouteTables, "RemoteRT.txt");
                }
            }

            public static Uri URI
            {
                get { return new Uri(Properties.Settings.Default.RemoteSource); }
            }

            public static Regex IPRegx
            {
                get
                {
                    //string expression = string.Format(@"^apnic\|{0}\|ipv4\|(\d+\.\d+\.\d+\.\d+)\|(\d+)\|$", Properties.Settings.Default.CurrentLocation);     
                    //apnic|CN|ipv4|  
                    string pattern = string.Format(@"apnic\|{0}\|ipv4\|(\d+\.\d+\.\d+\.\d+)\|(\d+)\|", Properties.Settings.Default.CurrentLocation);
                    return new Regex(pattern, RegexOptions.IgnoreCase); ;
                }
            }
        }

        public static class Template
        {
            static Template()
            {
                Directory.CreateDirectory(Properties.Settings.Default.Templates);
                Directory.CreateDirectory(Properties.Settings.Default.Outputs);
            }

            public static string TargetTemplate
            {
                get
                {
                    return System.IO.Path.Combine(Properties.Settings.Default.Templates, 
                        Properties.Settings.Default.CurrentPlatform);
                }
            }

            public static Regex CodeLineRegx
            {
                get
                {
                    string pattern = @"{{IPs}}([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,\ \{\}]*)\r\n";
                    return new Regex(pattern);
                }
            }
        }

        public static class TestData
        {
            static TestData()
            {
                Directory.CreateDirectory(Properties.Settings.Default.TestData);
            }

            public static string RemoteRouteTable
            {
                get
                {
                    return System.IO.Path.Combine(Properties.Settings.Default.TestData, "RemoteRT.txt");
                }
            }

            public static string TemplateFolder
            {
                get
                {
                    return System.IO.Path.Combine(Properties.Settings.Default.TestData,
                        "Templates",
                        Config.Platform);
                }
            }

            public static string TemplateFile
            {
                get
                {
                    return System.IO.Path.Combine(TemplateFolder,
                        "RoutesUp.bat");
                }
            }
        }
    }
}
