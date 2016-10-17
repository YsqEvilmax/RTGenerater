using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTGenerater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RTGenerater.Tests
{
    [TestClass()]
    public class ScriptGeneraterTests
    {
        private IList<IPNetwork> IPs = new List<IPNetwork>()
        {
            IPNetwork.Parse("27.125.204.0/22"),
            IPNetwork.Parse("27.126.64.0/18"),
            IPNetwork.Parse("27.126.128.0/20")
        };

        [TestMethod()]
        public void ExpandTest_Codeline_Expanded()
        {
            string codeline = @"{{IPs}}route add {{ip}} mask {{mask}} %gw% metric 5\r\n";

            string expectation = @"route add 27.125.204.0 mask 255.255.252.0 %gw% metric 5\r\n"
                + @"route add 27.126.64.0 mask 255.255.192.0 %gw% metric 5\r\n"
                + @"route add 27.126.128.0 mask 255.255.240.0 %gw% metric 5\r\n";

            ScriptGenerater sg = new ScriptGenerater();
            string result = sg.Expand(codeline, IPs);
            Assert.IsTrue(result.Equals(expectation));
        }

        [TestMethod()]
        public void CompositeTest_TemplateFile_Composited()
        {
            string expectation = "@echo off\r\n\r\n"
                + "for /F \"tokens=3\" %%* in ('route print ^| findstr \"\\<0.0.0.0\\>\"') do set \"gw=%%*\"\r\n\r\n"
                + "IF %gw%==%%* (\r\n  echo Error, connot find gateway\r\n  pause\r\n  exit\r\n)\r\n\r\n"
                + "ipconfig /flushdns\r\n\r\n@echo on\r\n\r\n"
                + "route add 27.125.204.0 mask 255.255.252.0 %gw% metric 5\r\n"
                + "route add 27.126.64.0 mask 255.255.192.0 %gw% metric 5\r\n"
                + "route add 27.126.128.0 mask 255.255.240.0 %gw% metric 5\r\n\r\n";

            ScriptGenerater sg = new ScriptGenerater();
            string result = sg.Composite(Config.TestData.TemplateFile,
                Config.Template.CodeLineRegx,
                IPs);
            Assert.IsTrue(result.Equals(expectation));
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompositeTest_NoTemplateFile_FileNotFoundException()
        {
            if (File.Exists(System.IO.Path.Combine(Config.Template.TargetTemplate, "RoutesUp.bat")))
            {
                File.Delete(System.IO.Path.Combine(Config.Template.TargetTemplate, "RoutesUp.bat"));
            }

            ScriptGenerater sg = new ScriptGenerater();
            sg.Composite(Config.Template.TargetTemplate, Config.Template.CodeLineRegx, IPs);
        }

        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void GenerateTest_NoTemplateDir_DirNotFoundException()
        {
            if (Directory.Exists(Config.Template.TargetTemplate))
            {
                Directory.Delete(Config.Template.TargetTemplate);
            }

            ScriptGenerater sg = new ScriptGenerater();
            sg.Generate(Config.Template.TargetTemplate, IPs, Config.Output);
        }

        [TestMethod()]
        public void GenerateTest_TestData_ScriptsGenerated()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Config.Output);
            foreach(FileInfo item in dirInfo.GetFiles()) { item.Delete(); }

            ScriptGenerater sg = new ScriptGenerater();
            sg.Generate(Config.TestData.TemplateFolder, IPs, Config.Output);

            Assert.IsTrue(File.Exists(System.IO.Path.Combine(Config.Output, "RoutesUp.bat")));
        }
    }
}