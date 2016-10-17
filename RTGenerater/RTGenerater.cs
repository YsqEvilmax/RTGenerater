using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace RouteTable
{
    public class RTGenerater
    {
        public RTGenerater()
        {
            IPs = new List<IPNetwork>();
        }

        public IList<IPNetwork> IPs { get; }


        public void FetchRemote(Uri from, string to)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(from, to);
            }
        }

        public void Optimize(string origin, Regex ipRegx)
        {
            if (!File.Exists(origin))
            {
                throw new FileNotFoundException("File: " + origin + " does not exist!");
            }

            using (FileStream rfs = new FileStream(origin, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(rfs))
                {
                    IPs.Clear();

                    string content = sr.ReadToEnd();

                    MatchCollection matches = ipRegx.Matches(content);
                    foreach (Match match in matches)
                    {
                        string ip = match.Groups[1].Value;
                        int masknum = int.Parse(match.Groups[2].Value);
                        int cidr = 32 - (int)Math.Log(masknum, 2);

                        IPNetwork ipNetwork = IPNetwork.Parse(ip + "/" + cidr.ToString());
                        IPs.Add(ipNetwork);
                    }
                }
            }
        }
    }
}
