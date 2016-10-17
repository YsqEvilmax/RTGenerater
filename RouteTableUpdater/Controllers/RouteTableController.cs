using RouteTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RouteTableUpdater.Controllers
{
    public class RouteTableController : Controller
    {
        private RTGenerater rtg = new RTGenerater();
        private ScriptGenerater stg = new ScriptGenerater();

        // GET: RouteTable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateSuperRouteTable()
        {
            rtg.FetchRemote(Config.Remote.URI, Config.Remote.Path);
            return View();
        }

        public ActionResult GenerateScript()
        {
            rtg.Optimize(Config.Remote.Path, Config.Remote.IPRegx);
            stg.Generate(Config.Template.TargetTemplate, rtg.IPs, Config.Output);
            return View();
        }
    }
}