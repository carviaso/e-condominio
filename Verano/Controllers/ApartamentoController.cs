using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Verano.Models;

namespace Verano.Controllers
{ 
    public class ApartamentoController : Controller
    {
        private VeranoEntities db = new VeranoEntities();

        //
        // GET: /Apartamento/
        
        [Authorize(Roles = "Administradores")]
        public ViewResult Index()
        {
            var apartamento = db.Apartamento.Include("Predio");
            return View(apartamento.ToList());
        }

        //
        // GET: /Apartamento/Details/5

        [Authorize(Roles = "Administradores")]
        public ViewResult Details(int id)
        {
            Apartamento apartamento = db.Apartamento.Single(a => a.Id == id);
            return View(apartamento);
        }

        //
        // GET: /Apartamento/Create
        [Authorize(Roles = "Administradores")]
        public ActionResult Create()
        {
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome");
            return View();
        } 

        //
        // POST: /Apartamento/Create
        [Authorize(Roles="Administradores")]
        [HttpPost]
        public ActionResult Create(Apartamento apartamento)
        {
            if (ModelState.IsValid)
            {
                db.Apartamento.AddObject(apartamento);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", apartamento.IdPredio);
            return View(apartamento);
        }
        
        //
        // GET: /Apartamento/Edit/5

        [Authorize(Roles = "Administradores")]
        public ActionResult Edit(int id)
        {
            Apartamento apartamento = db.Apartamento.Single(a => a.Id == id);
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", apartamento.IdPredio);
            return View(apartamento);
        }

        //
        // POST: /Apartamento/Edit/5
        [Authorize(Roles = "Administradores")]
        [HttpPost]
        public ActionResult Edit(Apartamento apartamento)
        {
            if (ModelState.IsValid)
            {
                db.Apartamento.Attach(apartamento);
                db.ObjectStateManager.ChangeObjectState(apartamento, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", apartamento.IdPredio);
            return View(apartamento);
        }

        //
        // GET: /Apartamento/Delete/5

        [Authorize(Roles = "Administradores")]
        public ActionResult Delete(int id)
        {
            Apartamento apartamento = db.Apartamento.Single(a => a.Id == id);
            return View(apartamento);
        }

        //
        // POST: /Apartamento/Delete/5
        [Authorize(Roles = "Administradores")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Apartamento apartamento = db.Apartamento.Single(a => a.Id == id);
            db.Apartamento.DeleteObject(apartamento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult CarregarCombo(int id)
        {
            var apartamento = db.Apartamento.Where(p => p.IdPredio == id).OrderBy(p => p.Numero);
            
            IList<Models.Forms.CarregarCombo> objs = new List<Models.Forms.CarregarCombo>();

            foreach (var item in apartamento)
            {
                Models.Forms.CarregarCombo obj = new Models.Forms.CarregarCombo();
                obj.Id = item.Id;
                obj.Numero = item.Numero;
                objs.Add(obj);
            }

            JsonResult result = new JsonResult();

            result.Data = objs;

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}