﻿using System.Linq;
using System.Web.Mvc;
using MvcContrib.Filters;
using Decats.Core;
using Raven.Client;
using MvcContrib;

namespace Decats.UI.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IDocumentSession _documentSession;

        public ItemsController(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        //
        // GET: /Items/List
        [HttpGet]
        public ActionResult List()
        {
            var items = _documentSession.Query<Item>()
                .Take(10)
                .ToArray();

            switch (items.Count())
            {
                case 0:
                    return View("NoItemsFound");
                case 1:
                    return this.RedirectToAction(c => c.Index(items.Single().Id));
                default:
                    return View(items);
            }
        }

        //
        // GET: /Items/
        [HttpGet]
        public ActionResult Index(string id)
        {
            var item = _documentSession.Load<Item>(id);

            return View(item);
        }

        //
        // GET: /Items/Create
        [HttpGet, ModelStateToTempData]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Items/Create

        [HttpPost, ModelStateToTempData]
        public ActionResult Create(Item input)
        {
            if (!ModelState.IsValid)
                return this.RedirectToAction(c => c.Create());



            _documentSession.Store(input);
            _documentSession.SaveChanges();

            return this.RedirectToAction(c => c.List());
        }
        
        //
        // GET: /Items/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Items/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Items/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
