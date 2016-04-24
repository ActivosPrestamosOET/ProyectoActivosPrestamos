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
        public ActionResult Index(string sortOrder, string currentFilter, string fechaSolicitud, string fechaRetiro, int? page)
        {
            System.Diagnostics.Trace.WriteLine(sortOrder);
            ViewBag.currentSort = sortOrder;//NO prestar atención
            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "numero_dsc" : "";//NO prestar atención
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";//NO prestar atención
            ViewBag.FDateSortParm = sortOrder == "FDate" ? "FDate_desc" : "FDate";
            ViewBag.PeriodoSortParm = sortOrder == "Periodo" ? "Periodo_desc" : "Periodo";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";

            if (fechaSolicitud != null || fechaRetiro != null)
            {
                page = 1;
            }
            else
            {
                fechaSolicitud = currentFilter;
            }

            ViewBag.CurrentFilter = fechaSolicitud;
            var prestamos = from s in db.PRESTAMOS
                            select s;

            if (!String.IsNullOrEmpty(fechaSolicitud) || !String.IsNullOrEmpty(fechaRetiro))
            {
                DateTime fechaS;
                DateTime fechaR;


                if (String.IsNullOrEmpty(fechaSolicitud))
                {
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Day == fechaR.Day);
                    }
                }
                else if (String.IsNullOrEmpty(fechaRetiro))
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Year == fechaS.Year 
                                                          && model.FECHA_SOLICITUD.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Day == fechaS.Day);
                    }                    
                }
                else
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Year == fechaS.Year
                                                          && model.FECHA_SOLICITUD.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Day == fechaS.Day);
                    }
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Day == fechaR.Day);
                    }                    
                }
            }

            switch (sortOrder)
            {
                case "numero_dsc":
                    prestamos = prestamos.OrderByDescending(s => s.NUMERO_BOLETA);
                    break;
                case "Date":
                    prestamos = prestamos.OrderBy(s => s.FECHA_SOLICITUD);
                    break;
                case "date_desc":
                    prestamos = prestamos.OrderByDescending(s => s.FECHA_SOLICITUD);
                    break;
                case "FDate":
                    prestamos = prestamos.OrderBy(s => s.FECHA_RETIRO);
                    break;
                case "FDate_desc":
                    prestamos = prestamos.OrderByDescending(s => s.FECHA_RETIRO);
                    break;
                case "Periodo":
                    prestamos = prestamos.OrderBy(s => s.PERIODO_USO);
                    break;
                case "Periodo_desc":
                    prestamos = prestamos.OrderByDescending(s => s.PERIODO_USO);
                    break;
                case "Name":
                    prestamos = prestamos.OrderBy(s => s.CED_SOLICITA);
                    break;
                case "Name_desc":
                    prestamos = prestamos.OrderByDescending(s => s.CED_SOLICITA);
                    break;
                default:  // Name ascending 
                    prestamos = prestamos.OrderBy(s => s.NUMERO_BOLETA);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(prestamos.ToPagedList(pageNumber, pageSize));
            //Hasta aquí paginación//                            
        }


        // GET: PRESTAMOes/Historial
        public ActionResult Historial(string CED_SOLICITA)
        {
            CED_SOLICITA = "PITAN0126052014.085230671";
            if (String.IsNullOrEmpty(CED_SOLICITA))
            {
                ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
                return View(db.PRESTAMOS.Where(model => model.Estado != 6));
            }
            else
            {
                return View(db.PRESTAMOS.Where(model => model.CED_SOLICITA == CED_SOLICITA && model.Estado != 6));
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
                        where o.ID == id
                        select new { USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                var t = new Tuple<string>(m.USUARIO);
                ViewBag.Nombre = t.Item1;
            }



           /* List<String> solicitantes = new List<String>();
            List<String> ceds = new List<String>();
            foreach (Activos_PrestamosOET.Models.PRESTAMO p in db.PRESTAMOS)
            {
                foreach (Activos_PrestamosOET.Models.USUARIO u in db.USUARIOS)
                {
                    if (p.USUARIO != null)
                    {
                        if (p.USUARIO.Equals(u.IDUSUARIO))
                        {
                            solicitantes.Add(u.NOMBRE);
                            ceds.Add(u.IDUSUARIO);
                        }
                    }
                }
            }



            var lista1 = from o in db.PRESTAMOS
                         from o2 in db.EQUIPO_SOLICITADO
                         where o.ID == o2.ID_PRESTAMO
                         select new { EQUIPO_SOLICITADO = o2.TIPO_ACTIVO, EQUIPO_SOLICITADO_CANTIDAD = o2.CANTIDAD };

            List<Tuple<string, decimal>> l1 = new List<Tuple<string, decimal>>();
            foreach (var m in lista1)
            {
                var t1 = new Tuple<string, decimal>(m.EQUIPO_SOLICITADO, m.EQUIPO_SOLICITADO_CANTIDAD);
                l1.Add(t1);
            }

            ViewBag.Equipo_Solict = l1;
            */
            return View(pRESTAMO);
        }

        // GET: PRESTAMOes/Create
        public ActionResult Create()
        {
            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");



            List<String> categorias = new List<string>();

            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select t.NOMBRE).Distinct();

            foreach (var c in cat)
            {
                categorias.Add(c.ToString());
            }

            ViewBag.CATEGORIAS = categorias;

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
        public ActionResult Edit([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO p, string id)
        {
            PRESTAMO P = db.PRESTAMOS.Find(id);
            //P.ID = p.ID;
            P.MOTIVO = p.MOTIVO;
            //P.NUMERO_BOLETA = p.NUMERO_BOLETA;
            // P.NUMERO_BOLETA = db.PRESTAMOS.;//context.Persons.Max(p => p.Age); ;
            //P.OBSERVACIONES_APROBADO = "";
            //P.OBSERVACIONES_RECIBIDO = "";
            P.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
            P.PERIODO_USO = p.PERIODO_USO;
            P.SIGLA_CURSO = p.SIGLA_CURSO;
            //P.CED_APRUEBA = p.CED_APRUEBA;
            //P.CED_SOLICITA = p.CED_SOLICITA;
            P.FECHA_RETIRO = p.FECHA_RETIRO;
            //P.FECHA_SOLICITUD = System.DateTimeOffset.Now.Date;//SELECT SYSDATE FROM DUAL
            P.SOFTWARE_REQUERIDO = "";
            P.Estado = 1;
            if (ModelState.IsValid)
            {
                db.Entry(P).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Historial");
            }
            return View(P);
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
            pRESTAMO.Estado = 6;
            db.SaveChanges();
            return RedirectToAction("Historial");
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
