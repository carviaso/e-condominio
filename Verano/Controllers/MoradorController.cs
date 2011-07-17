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
    public class MoradorController : Controller
    {
        private VeranoEntities db = new VeranoEntities();

        //
        // GET: /Morador/

        [Authorize]
        public ActionResult Index(int pagina = 1)
        {
            var morador = db.Morador.FirstOrDefault(m => m.Login == this.HttpContext.User.Identity.Name);

            if (morador == null)
            {
                return RedirectToAction("Create");
            }

            if (this.User.IsInRole("Administrador"))
            {
                ViewData["Total"] = db.Morador.Count();

                var moradores = db.Morador.Skip((pagina - 1) * 30).Take(30);
                
                return View(moradores.ToList());
            }
            else
            {
                var moradores = db.Morador.Where(m => m.Login == this.HttpContext.User.Identity.Name);
                return View(moradores.ToList());
            }
        }

        //
        // GET: /Morador/Details/5
        
        [Authorize]
        public ViewResult Details(int id)
        {
            Morador morador = db.Morador.Single(m => m.Id == id);
            return View(morador);
        }

        //
        // GET: /Morador/Create

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome");
            ViewBag.IdApartamento = new SelectList(db.Apartamento.Where(ap => ap.IdPredio == 1), "Id", "Numero");
            return View();
        } 

        //
        // POST: /Morador/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Morador morador)
        {
            if (ModelState.IsValid)
            {
                morador.DataCadastro = DateTime.Now;
                db.Morador.AddObject(morador);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return RedirectToAction("Index", "Pergunta");

            //ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", morador.Apartamento.IdPredio);
            //ViewBag.IdApartamento = new SelectList(db.Apartamento.Where(ap => ap.IdPredio == 1), "Id", "Numero", morador.IdApartamento);
            //return View(morador);
        }
        
        //
        // GET: /Morador/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            Morador morador = db.Morador.Single(m => m.Id == id);
            if (morador == null)
            {
                throw new HttpException(404, "Morador não encontrado.");
            }
            if (morador.Login != this.HttpContext.User.Identity.Name)
            {
                throw new HttpException(403, "O usuário logado não pode modificar esse morador.");
            }
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", morador.Apartamento.IdPredio);
            ViewBag.IdApartamento = new SelectList(db.Apartamento.Where(ap => ap.IdPredio == 1), "Id", "Numero", morador.IdApartamento);
            return View(morador);
        }

        //
        // POST: /Morador/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Morador morador)
        {
            if (ModelState.IsValid)
            {
                db.Morador.Attach(morador);
                db.ObjectStateManager.ChangeObjectState(morador, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPredio = new SelectList(db.Predio, "Id", "Nome", morador.Apartamento.IdPredio);
            ViewBag.IdApartamento = new SelectList(db.Apartamento.Where(ap => ap.IdPredio == 1), "Id", "Numero", morador.IdApartamento);
            return View(morador);
        }

        //
        // GET: /Morador/Delete/5

        [Authorize(Roles="Administradores")]
        public ActionResult Delete(int id)
        {
            Morador morador = db.Morador.Single(m => m.Id == id);
            return View(morador);
        }

        //
        // POST: /Morador/Delete/5
        [Authorize(Roles="Administradores")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Morador morador = db.Morador.Single(m => m.Id == id);
            db.Morador.DeleteObject(morador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}