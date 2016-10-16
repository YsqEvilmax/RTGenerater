using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTGenerater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTGenerater.Tests
{
    [TestClass()]
    public class RTUpdaterTests
    {
        [TestMethod()]
        public void FetchRemoteTest()
        {
            string filePath = System.IO.Path.Combine("./RouteTables", "RemoteIPs.txt");
            if (File.Exists(filePath)){
                File.Delete(filePath);
            }

            RTUpdater rtu = new RTUpdater();
            rtu.FetchRemote();

            Assert.IsTrue(File.Exists(filePath));
        }
    }
}