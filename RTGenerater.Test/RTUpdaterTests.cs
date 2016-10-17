using Microsoft.VisualStudio.TestTools.UnitTesting;
using RouteTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RouteTable.Tests
{
    [TestClass()]
    public class RTUpdaterTests
    {
        [TestMethod()]
        public void FetchRemoteTest()
        {
            RTGenerater rtu = new RTGenerater();

            if (File.Exists(Config.Remote.Path))
            {
                File.Delete(Config.Remote.Path);
            }

            rtu.FetchRemote(Config.Remote.URI, Config.Remote.Path);

            Assert.IsTrue(File.Exists(Config.Remote.Path));
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void OptimizeTest_NoOriginalFile_FileNotFoundException()
        {
            if (File.Exists(Config.Remote.Path))
            {
                File.Delete(Config.Remote.Path);
            }

            RTGenerater rtu = new RTGenerater();
            rtu.Optimize(Config.Remote.Path, Config.Remote.IPRegx);
        }

        [TestMethod()]
        public void OptimizeTest_TestData_GetIPNetworks()
        {
            if (!File.Exists(Config.TestData.RemoteRouteTable))
            {
                throw new FileNotFoundException("File :" + Config.TestData.RemoteRouteTable + " does not exist");
            }

            RTGenerater rtu = new RTGenerater();
            rtu.Optimize(Config.TestData.RemoteRouteTable, Config.Remote.IPRegx);

            if(Config.Location == "NZ")
            {
                Assert.AreEqual(rtu.IPs.Count, 1);
                Assert.AreEqual(rtu.IPs[0], IPNetwork.Parse("27.123.20.0/22"));
            }
            else if (Config.Location == "JP")
            {
                Assert.AreEqual(rtu.IPs.Count, 3);
                Assert.AreEqual(rtu.IPs[0], IPNetwork.Parse("27.125.204.0/22"));
                Assert.AreEqual(rtu.IPs[1], IPNetwork.Parse("27.126.64.0/18"));
                Assert.AreEqual(rtu.IPs[2], IPNetwork.Parse("27.126.128.0/20"));
            } 

        }
    }
}