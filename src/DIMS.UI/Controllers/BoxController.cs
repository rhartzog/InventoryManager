using System.Web.Mvc;
using AutoMapper;
using DIMS.Core.Entities;
using DIMS.Data.DataAccess;
using DIMS.UI.Models.Edit;

namespace DIMS.UI.Controllers
{
    public class BoxController : Controller
    {
        private readonly IRepository<Box> _boxRepository;

        public BoxController(IRepository<Box> boxRepository)
        {
            _boxRepository = boxRepository;
        }

        [HttpPost]
        public ActionResult Create(BoxForm form)
        {
            var test = "test";

            return View();
        }
    }
}