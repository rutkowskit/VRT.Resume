using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    public class ErrorHandlerController : Controller
    {        
        [OutputCache(NoStore =true)]
        public ActionResult Index()
        {
            return View();
        }
    }
}