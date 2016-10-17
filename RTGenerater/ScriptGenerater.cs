using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RouteTable
{
    public class ScriptGenerater
    {
        public void Generate(string templateDir, IList<IPNetwork> IPs, string outDir)
        {
            if (!Directory.Exists(templateDir))
            {
                throw new DirectoryNotFoundException("Directory: " + templateDir + "cannot be found!");
            }
            DirectoryInfo dirInfo = new DirectoryInfo(templateDir);
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {

                string content = Composite(fileInfo.FullName, Config.Template.CodeLineRegx, IPs);

                using (FileStream ofs = new FileStream(System.IO.Path.Combine(outDir, fileInfo.Name),
                    FileMode.Create))
                {
                    using (StreamWriter osw = new StreamWriter(ofs))
                    {
                        osw.WriteLine(content);
                    }
                }
            }
        }

        public string Composite(string template, Regex codeline, IList<IPNetwork> IPs)
        {
            if (!File.Exists(template))
            {
                throw new FileNotFoundException("File: " + template + " does not exist!");
            }
            string code = "";
            using (FileStream tfs = new FileStream(template, FileMode.Open))
            {
                using (StreamReader tsr = new StreamReader(tfs))
                {
                    string content = tsr.ReadToEnd();

                    MatchCollection matches = codeline.Matches(content);

                    foreach (Match item in matches)
                    {
                        content = content.Replace(item.Value, Expand(item.Value, IPs));
                    }
                    code = content;
                }
            }

            return code;
        }

        public string Expand(string codeline, IList<IPNetwork> IPs)
        {
            string result = "";
            foreach (IPNetwork item in IPs)
            {
                string temp = codeline;
                temp = temp.Replace("{{IPs}}", "");
                temp = temp.Replace("{{ip}}", item.Network.ToString());
                temp = temp.Replace("{{mask}}", item.Netmask.ToString());
                result += temp;
            }
            return result;
        }
    }
}
