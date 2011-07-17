using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Verano.Models;
using Verano.Models.Forms;

namespace Verano.Controllers
{ 
    public class PerguntaController : Controller
    {
        private VeranoEntities db = new VeranoEntities();

        //
        // GET: /Pergunta/

        [Authorize(Roles="Administrador")]
        public ViewResult Index()
        {
            return View(db.Pergunta.ToList());
        }

        [Authorize]
        public ActionResult Respostas(int id)
        {
            ViewData["IdMorador"] = id;

            var morador = db.Morador.FirstOrDefault(m => m.Id == id);
            var respostas = db.Resposta.Where(r => r.IdMorador == id);

            if (!respostas.Any())
            {
                return RedirectToAction("Responder", new { @id = id});
            }

            ViewData["Perguntas"] = db.Pergunta.Include("Opcao");
            return View(respostas.ToList());
        }

        [Authorize]
        [HttpGet]
        public ViewResult Responder(int id)
        {
            ViewData["IdMorador"] = id;
            return View(db.Pergunta.FirstOrDefault());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Responder(Resposta resposta)
        {
            ViewData["IdMorador"] = resposta.IdMorador;

            Pergunta atual = db.Pergunta.FirstOrDefault(p => p.Id == resposta.IdPergunta);

            Resposta existe = atual.Resposta.FirstOrDefault(r => r.IdMorador == resposta.IdMorador);
            if (existe != null)
            {
                existe.IdOpcao = resposta.IdOpcao;
                existe.Observacao = resposta.Observacao;
                db.SaveChanges();
                return RedirectToAction("Respostas", "Pergunta", new { @id = resposta.IdMorador});
            }
            else
            {
                db.Resposta.AddObject(resposta);
                db.SaveChanges();
            }

            Pergunta proxima = db.Pergunta.FirstOrDefault(p => p.Id == resposta.IdPergunta + 1);

            if (proxima != null)
            {
                return View(proxima);
            }
            else
            {
                return View("Final");
            }
        }

        //
        // GET: /Pergunta/Details/5

        [Authorize(Roles = "Administradores")]
        public ViewResult Details(int id)
        {
            Pergunta pergunta = db.Pergunta.Single(p => p.Id == id);
            return View(pergunta);
        }

        //
        // GET: /Pergunta/Create

        [Authorize(Roles = "Administradores")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Pergunta/Create

        [Authorize(Roles = "Administradores")]
        [HttpPost]
        public ActionResult Create(Pergunta pergunta)
        {
            if (ModelState.IsValid)
            {
                db.Pergunta.AddObject(pergunta);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(pergunta);
        }
        
        //
        // GET: /Pergunta/Edit/5

        [Authorize(Roles = "Administradores")]
        public ActionResult Edit(int id)
        {
            Pergunta pergunta = db.Pergunta.Single(p => p.Id == id);
            return View(pergunta);
        }

        //
        // POST: /Pergunta/Edit/5

        [Authorize(Roles = "Administradores")]
        [HttpPost]
        public ActionResult Edit(Pergunta pergunta)
        {
            if (ModelState.IsValid)
            {
                db.Pergunta.Attach(pergunta);
                db.ObjectStateManager.ChangeObjectState(pergunta, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pergunta);
        }

        //
        // GET: /Pergunta/Delete/5

        [Authorize(Roles = "Administradores")]
        public ActionResult Delete(int id)
        {
            Pergunta pergunta = db.Pergunta.Single(p => p.Id == id);
            return View(pergunta);
        }

        //
        // POST: /Pergunta/Delete/5

        [Authorize(Roles = "Administradores")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Pergunta pergunta = db.Pergunta.Single(p => p.Id == id);
            db.Pergunta.DeleteObject(pergunta);
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