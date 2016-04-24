using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Activos_PrestamosOET.Models;
using System.Data;
using System.Web.UI.WebControls;
using System;
using PagedList;
using System.Globalization;
using System.Collections.Generic;


namespace Activos_PrestamosOET.Controllers
{

    public class PRESTAMOesController : Controller
    {
        private PrestamosEntities db = new PrestamosEntities();

        // GET: PRESTAMOes
        public ActionResult Index(string fechaSolicitud, string fechaRetiro, string sortOrder, string currentFilter, int? page)
        {

            ViewBag.currentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "numero_dsc" : "";
            ViewBag.DateSortParm = sortOrder == "fecha_solicitud" ? "date_desc" : "Date";
            if (String.IsNullOrEmpty(fechaSolicitud) && String.IsNullOrEmpty(fechaRetiro))
                return View(db.PRESTAMOS.ToList());
            else
            {
                DateTime fechaS;
                DateTime fechaR;


                if (String.IsNullOrEmpty(fechaSolicitud))
                {
                    if (!DateTime.TryParse(fechaRetiro, out fechaR))
                    {
                        return View(db.PRESTAMOS.ToList());
                    }
                    return View(db.PRESTAMOS.Where(model => model.FECHA_RETIRO == fechaR.Date));
                }
                else if (String.IsNullOrEmpty(fechaRetiro))
                {
                    if (!DateTime.TryParseExact(fechaSolicitud, "dd/mm/yyyy",new CultureInfo("es"),DateTimeStyles.None, out fechaS))
                    {
                        return View(db.PRESTAMOS.ToList());
                    }
                    return View(db.PRESTAMOS.Where(model => model.FECHA_SOLICITUD == fechaS.Date));
                }
                else
                {
                    if (!DateTime.TryParseExact(fechaSolicitud, "dd/mm/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        return View(db.PRESTAMOS.ToList());
                    }
                    if (!DateTime.TryParse(fechaRetiro, out fechaR))
                    {
                        return View(db.PRESTAMOS.ToList());
                    }
                    return View(db.PRESTAMOS.Where(model => model.FECHA_RETIRO == fechaR.Date && model.FECHA_SOLICITUD == fechaS.Date));
                }
            }
        }


        // GET: PRESTAMOes/Historial
        public ActionResult Historial(string CED_SOLICITA)
        {
            CED_SOLICITA = "PITAN0126052014.085230671";
            if (String.IsNullOrEmpty(CED_SOLICITA))
            {
                ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
                return View(db.PRESTAMOS.ToList());
            }
            else
            {
                return View(db.PRESTAMOS.Where(model => model.CED_SOLICITA == CED_SOLICITA));
            }
        }

        // GET: PRESTAMOes/Detalles
        public ActionResult Detalles(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }
            return View(pRESTAMO);
        }







        // GET: PRESTAMOes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
           // ViewBag.clear();

            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }

            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.CED_SOLICITA == o2.IDUSUARIO
                        select new { PRESTAMO = o, USUARIO = o2.NOMBRE};

            List<Tuple<Activos_PrestamosOET.Models.PRESTAMO, string>> l = new List<Tuple<Activos_PrestamosOET.Models.PRESTAMO, string>>();
            foreach (var m in lista)
            {
                var t = new Tuple<Activos_PrestamosOET.Models.PRESTAMO, string>(m.PRESTAMO, m.USUARIO);
                l.Add(t);
                ViewBag.Nombre = t.Item2;
            }

            var lista1 = from o in db.PRESTAMOS
                        from o2 in db.EQUIPO_SOLICITADO
                        where o.ID == o2.ID_PRESTAMO
                        select new {EQUIPO_SOLICITADO = o2.TIPO_ACTIVO, EQUIPO_SOLICITADO_CANTIDAD = o2.CANTIDAD };

            List<Tuple< string, decimal>> l1 = new List<Tuple<string, decimal>>();
            foreach (var m in lista1)
            {
                var t1 = new Tuple< string, decimal>(m.EQUIPO_SOLICITADO, m.EQUIPO_SOLICITADO_CANTIDAD);
                l1.Add(t1);
            }

            ViewBag.Equipo_Solict = l1;

            return View(pRESTAMO);
        }

        // GET: PRESTAMOes/Create
        public ActionResult Create()
        {
            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
            return View();
        }

        // POST: PRESTAMOes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,SIGLA_CURSO,Estado,CED_SOLICITA,CED_APRUEBA")] PRESTAMO p)
        {
            //p.FECHA_RETIRO
            PRESTAMO P = new PRESTAMO();
            if (ModelState.IsValid)
            {
                
                P.ID = p.ID;
                P.MOTIVO = p.MOTIVO;
                //P.NUMERO_BOLETA = p.NUMERO_BOLETA;
               // P.NUMERO_BOLETA = db.PRESTAMOS.;//context.Persons.Max(p => p.Age); ;
                P.OBSERVACIONES_APROBADO = "";
                P.OBSERVACIONES_RECIBIDO = "";
                P.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
                P.PERIODO_USO = p.PERIODO_USO;
                P.SIGLA_CURSO = p.SIGLA_CURSO;
                P.CED_APRUEBA = p.CED_APRUEBA;
                P.CED_SOLICITA = p.CED_SOLICITA;
                P.FECHA_RETIRO = p.FECHA_RETIRO;
                P.FECHA_SOLICITUD = System.DateTimeOffset.Now.Date;//SELECT SYSDATE FROM DUAL
                P.SOFTWARE_REQUERIDO = "";
                P.Estado = 1;
                //P.CED_APRUEBA = p.CED_APRUEBA;
                db.PRESTAMOS.Add(P);
                db.SaveChanges();
                return RedirectToAction("Historial");
            }

            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_SOLICITA);
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_APRUEBA);
            return View(P);
        }











        /*
        // GET: PRESTAMOes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PRESTAMOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO pRESTAMO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.PRESTAMOS.Add(pRESTAMO);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "No se pudo realizar la solicitud. Por favor trate nuevamente y si el problema persiste comuníquese con el administrador") ;
            }

            return View(pRESTAMO);
        }
        */

        // GET: PRESTAMOes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }
            return View(pRESTAMO);
        }

        // POST: PRESTAMOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO pRESTAMO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pRESTAMO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pRESTAMO);
        }

        // GET: PRESTAMOes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }
            return View(pRESTAMO);
        }

        // POST: PRESTAMOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            db.PRESTAMOS.Remove(pRESTAMO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public enum Estadito
        {
            //Todos,
            Pendiente,
            Aprobado,
            Denegado,
            Cancelado
        }
    }
}
