using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Decats.Core;
using Raven.Client;

namespace Decats.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDocumentSession _session;

        public HomeController(IDocumentSession session)
        {
            _session = session;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            var boxes = _session.Query<Box>();

            string list = string.Empty;
            foreach (var box in boxes)
            {
                list = string.Format("{0},", box.Id);
            }

            //var box = _session.Load<Box>(1);
            //box.RelocateToNewCampus("St. Pius");

            //_session.Store(box);
            //_session.SaveChanges();

            return Content(list);
        }

    }
}
