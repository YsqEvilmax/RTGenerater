using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace RTGenerater
{
    public class RTUpdater
    {
        public RTUpdater()
        {
            IPs = new List<IPNetwork>();

            if (!Directory.Exists(Properties.Settings.Default.RouteTables))
            {
                Directory.CreateDirectory(Properties.Settings.Default.RouteTables);
            }
        }

        public ICollection<IPNetwork> IPs { get; }

        private string RemoteIPTable
        {
            get
            {
                return System.IO.Path.Combine(Properties.Settings.Default.RouteTables, "RemoteIPs.txt");
            }
        }

        private string Pattern
        {
            get
            {
                return string.Format(@"apnic\| {0}\| ipv4\| ([\d\.] +)\| (\d +)\|", Properties.Settings.Default.CurrentLocation);
            }
        }

        private string CurrentIPTable
        {
            get
            {
                return System.IO.Path.Combine(Properties.Settings.Default.RouteTables, "CurrentLocationIPs.txt");
            }
        }

        public void FetchRemote()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(new System.Uri(Properties.Settings.Default.RemoteSource),
                    RemoteIPTable);
            }
        }

        public void MakeTargetRT()
        {
            if (!File.Exists(RemoteIPTable))
            {
                throw new FileNotFoundException("File" + RemoteIPTable + "does not exist!");
            }

            using (FileStream rfs = new FileStream(RemoteIPTable, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(rfs))
                {
                    IPs.Clear();

                    string content = sr.ReadToEnd();

                    Regex rgx = new Regex(Pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(content);
                    foreach (Match match in matches)
                    {
                        string ip = match.Captures[1].Value;
                        int masknum = int.Parse(match.Captures[2].Value);
                        int cidr = 32 - (int)Math.Log(masknum);

                        IPNetwork ipNetwork = IPNetwork.Parse(ip + "/" + cidr.ToString());
                        IPs.Add(ipNetwork);
                    }
                }
            }
        }
    }
}
