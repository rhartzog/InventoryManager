using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib;

namespace Decats.UI.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Admin/Default/

        public ActionResult Index()
        {
            return this.RedirectToAction<ItemsController>(c => c.List());
        }
    }
}
