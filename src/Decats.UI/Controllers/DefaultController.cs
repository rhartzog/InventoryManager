using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Decats.Core;
using Raven.Client;

namespace Decats.UI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IDocumentSession _session;

        public DefaultController(IDocumentSession session)
        {
            _session = session;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
