using AscentrikProjectWorkflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tesseris.Web.SimpleSecurity;

namespace AscentrikProjectWorkflow.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/
        internal readonly DashboardModel dashboardModel = new DashboardModel();
        public ActionResult Index()
        {
            var result = SimpleSecurityProvider.Current.Register("kangude.sandeep@gmail.com", "password", "1", "6", true);
            return View();
        }

    }
}
