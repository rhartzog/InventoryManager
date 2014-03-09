using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIMS.Core;
using DIMS.Core.Entities;
using DIMS.Data.DataAccess;

namespace DIMS.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Box> _repository;

        public HomeController(IRepository<Box> repository)
        {
            _repository = repository;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
    }
}
