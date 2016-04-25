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

        protected static int consecutivo;
        protected String generarID()
        {
            consecutivo = (consecutivo + 1) % 999;
            return ""
            + DateTime.Now.Day.ToString("D2")
            + DateTime.Now.Month.ToString("D2")
            + DateTime.Now.Year.ToString()
            + DateTime.Now.Hour.ToString("D2")
            + DateTime.Now.Minute.ToString("D2")
            + DateTime.Now.Second.ToString("D2")
            + DateTime.Now.Millisecond.ToString("D3")
            + consecutivo.ToString("D3");
        }


        // GET: PRESTAMOes
        public ActionResult Index(string sortOrder, string currentFilter, string fechaSolicitud, string fechaRetiro, string estado, int? page)
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
            var prestamos = db.PRESTAMOS.Include(i => i.USUARIO);

            if (!String.IsNullOrEmpty(fechaSolicitud) || !String.IsNullOrEmpty(fechaRetiro))
            {
                DateTime fechaS;
                DateTime fechaR;


                if (String.IsNullOrEmpty(fechaSolicitud))
                {
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Value.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Value.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Value.Day == fechaR.Day);
                    }
                }
                else if (String.IsNullOrEmpty(fechaRetiro))
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Value.Year == fechaS.Year
                                                          && model.FECHA_SOLICITUD.Value.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Value.Day == fechaS.Day);
                    }
                }
                else
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Value.Year == fechaS.Year
                                                          && model.FECHA_SOLICITUD.Value.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Value.Day == fechaS.Day);
                    }
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Value.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Value.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Value.Day == fechaR.Day);
                    }
                }
            }
            if (!string.IsNullOrEmpty(estado) && estado != "0")
            {
                int est = int.Parse(estado);
                var int16 = Convert.ToInt16(est);
                prestamos = prestamos.Where(model => model.Estado == int16);
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
        /*public ActionResult Historial(string CED_SOLICITA)
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
        }*/


        
        // GET: PRESTAMOes/Historial
        public ActionResult Historial(string CED_SOLICITA, string currentFilter, string estado, int? page)
        {
            CED_SOLICITA = "PITAN0126052014.085230671";

            var prestamos = from s in db.PRESTAMOS
                            select s;
            var p = prestamos.Where(model => model.CED_SOLICITA == CED_SOLICITA);
            if (!string.IsNullOrEmpty(estado) && estado != "0")
            {
                int est = int.Parse(estado);
                var int16 = Convert.ToInt16(est);
                var prestamosFiltrados = p.Where(model => model.Estado == int16);
                prestamosFiltrados = prestamosFiltrados.OrderByDescending(s => s.FECHA_SOLICITUD);
                int pageSize1 = 5;
                int pageNumber1 = (page ?? 1);
                return View(prestamosFiltrados.ToPagedList(pageNumber1, pageSize1));
            }
            else
            {
                var prestamosFiltrados = p.Where(model => model.Estado != 6);
                prestamosFiltrados = prestamosFiltrados.OrderByDescending(s => s.FECHA_SOLICITUD);
                prestamos = prestamos.OrderByDescending(s => s.FECHA_SOLICITUD);
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(prestamos.ToPagedList(pageNumber, pageSize));
            }
        }

        public string viewBagFechaSolicitada(DateTime sol)
        {
            string f = sol.Date.ToShortDateString();
            f=f.Replace("/20","/");
            return f;
        }
        // GET: PRESTAMOes/Detalles
        public ActionResult Detalles(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            ViewBag.fechSol = viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD.Value.Date);
            //viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD);

            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }

            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.ID == id
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                if (m.Prestamo.ID == id)
                {
                    if (m.Prestamo.CED_SOLICITA == m.CEDULA)
                    {
                        var t = new Tuple<string>(m.USUARIO);
                        ViewBag.Nombre = t.Item1;
                    }
                }
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
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                if (m.Prestamo.ID == id)
                {
                    if (m.Prestamo.CED_SOLICITA == m.CEDULA)
                    {
                        var t = new Tuple<string>(m.USUARIO);
                        ViewBag.Nombre = t.Item1;
                    }
                }
            }
            /*  -------------------------------------------------------------------------------------------  */
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();

            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where o.ID == id
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD };


            var equipo = new List<List<String>>();
            foreach (var x in equipo_sol)
            {
                if (x.ID == id)
                {
                    if (x.ID == x.ID_EQUIPO)
                    {


                        List<String> temp = new List<String>();
                        if (x.TIPO != null)
                        {
                            foreach (var y in cat)
                            {

                                if (x.TIPO == y.ID.ToString())
                                {

                                    temp.Add(y.NOMBRE);
                                    break;
                                }
                                
                            }
                        }
                        else
                        {
                            temp.Add("");

                        }


                        if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add(""); }

                        equipo.Add(temp);
                    }
                }
            }

            ViewBag.Equipo_Solict = equipo;

            /*  -------------------------------------------------------------------------------------------  */

            /* 
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



            List<String> categorias = new List<String>();

            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select t.NOMBRE).Distinct();

            foreach (String c in cat)
            {
                categorias.Add(c.ToString());
            }

            ViewData["categorias"] = categorias;
            TempData["categorias"] = categorias;

            return View();
        }


        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,SIGLA_CURSO,Estado,CED_SOLICITA,CED_APRUEBA")] PRESTAMO p, int[] Cantidad)
        {
            //p.FECHA_RETIRO
            PRESTAMO prestamo = new PRESTAMO();
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                //P.ID = p.ID;
                prestamo.ID = generarID();
                prestamo.MOTIVO = p.MOTIVO;
                prestamo.OBSERVACIONES_APROBADO = "";
                prestamo.OBSERVACIONES_RECIBIDO = "";
                prestamo.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
                prestamo.PERIODO_USO = p.PERIODO_USO;
                prestamo.SIGLA_CURSO = p.SIGLA_CURSO;
                prestamo.CED_APRUEBA = p.CED_APRUEBA;
                prestamo.CED_SOLICITA = p.CED_SOLICITA;
                prestamo.FECHA_RETIRO = p.FECHA_RETIRO;
                prestamo.FECHA_SOLICITUD = System.DateTimeOffset.Now.Date;//SELECT SYSDATE FROM DUAL
                prestamo.SOFTWARE_REQUERIDO = p.SOFTWARE_REQUERIDO;
                prestamo.Estado = 1;
                db.PRESTAMOS.Add(prestamo);
                db.SaveChanges();
                foreach (int c in Cantidad)
                {
                    EQUIPO_SOLICITADO equipo = new EQUIPO_SOLICITADO();
                    equipo.CANTIDAD = c;
                    equipo.TIPO_ACTIVO = "13";
                    equipo.ID_PRESTAMO = prestamo.ID;
                    db.EQUIPO_SOLICITADO.Add(equipo);
                    db.SaveChanges();
                }

                return RedirectToAction("Historial");
            }

            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_SOLICITA);
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_APRUEBA);
            return View(prestamo);
        }


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
            ViewBag.fechSol = viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD.Value.Date);
            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.ID == id
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                if (m.Prestamo.ID == id)
                {
                    if (m.Prestamo.CED_SOLICITA == m.CEDULA)
                    {
                        var t = new Tuple<string>(m.USUARIO);
                        ViewBag.Nombre = t.Item1;
                    }
                }
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
            ViewBag.fechSol = P.FECHA_SOLICITUD.Value.ToShortDateString();
            P.MOTIVO = p.MOTIVO;
            P.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
            P.PERIODO_USO = p.PERIODO_USO;
            P.SIGLA_CURSO = p.SIGLA_CURSO;
            P.FECHA_RETIRO = p.FECHA_RETIRO;
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
            ViewBag.fechSol = viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD.Value.Date);
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }

            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.ID == id
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                if (m.Prestamo.ID == id)
                {
                    if (m.Prestamo.CED_SOLICITA == m.CEDULA)
                    {
                        var t = new Tuple<string>(m.USUARIO);
                        ViewBag.Nombre = t.Item1;
                    }
                }
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

        public void aceptar_solicitud(string id, List<decimal> cantidad_apovada)
        {
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where o.ID == id
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD };

            foreach (var x in equipo_sol)
            {
                if (x.ID == id)
                {
                    if (x.ID == x.ID_EQUIPO)
                    {

                        EQUIPO_SOLICITADO P = db.EQUIPO_SOLICITADO.Find(id, x.TIPO, x.CANTIDAD);

                        decimal temp = cantidad_apovada.First();

                        P.CANTIDADAPROBADA = temp;
                        if (ModelState.IsValid)
                        {
                            db.Entry(P).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        cantidad_apovada.Remove(cantidad_apovada.First());
                    }


                }
            }

        }
        public enum Estadito
        {
            //Todos,
            Pendiente,
            Aprobada,
            Denegada,
            Abierta,
            Cerrada,
            Cancelada
        }
    }
}
