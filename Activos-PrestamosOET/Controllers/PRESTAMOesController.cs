﻿using System.Data.Entity;
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

        protected String traerCategoria( String tipo )
        {
            var consultaCat = from t in db.TIPOS_ACTIVOS
                      where t.NOMBRE.Equals(tipo)
                      select t.ID;

            List<String> categorias = new List<String>();

            foreach (int c in consultaCat)
            {
                categorias.Add(c.ToString());
            }
            String cat = categorias[0];

            return cat;
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
        public ActionResult Historial(string CED_SOLICITA, string currentFilter, string estado, int? page)
        {
            CED_SOLICITA = "PITAN0126052014.085230671";
            ViewBag.estado = estado;
            ViewBag.mensajeConfirmacion = (String)TempData["confirmacion"];

            var prestamos = from s in db.PRESTAMOS where s.CED_SOLICITA == CED_SOLICITA select s ;
            int est;
            if (string.IsNullOrEmpty(estado)) {
                est = 0;
            } else {
                est = int.Parse(estado);
            }
            if (!string.IsNullOrEmpty(estado) && estado != "0" )
            {
                var int16 = Convert.ToInt16(est);
                prestamos = prestamos.Where(model => model.Estado == int16);
            }
            //prestamos = prestamos.OrderByDescending(s => s.FECHA_RETIRO);
            prestamos = prestamos.OrderByDescending(s => s.FECHA_SOLICITUD);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(prestamos.ToPagedList(pageNumber, pageSize));
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

            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();

            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };

            var equipo = new List<List<String>>();

            foreach (var y in cat)
            {
                bool existeCategoria = false;
                List<String> temp = new List<String>();
                foreach (var x in equipo_sol)
                {

                    if (x.TIPO != null)
                    {
                        if (x.TIPO == y.ID.ToString())
                        {

                            temp.Add(y.NOMBRE);
                            existeCategoria = true;
                            if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add("0"); }
                            if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add("0"); }
                            break;
                        }
                    }
                    else
                    {
                        temp.Add("");
                        temp.Add("");
                        temp.Add("");
                    }
                }
                if (!existeCategoria)
                {
                    temp.Add(y.NOMBRE);
                    temp.Add("0");
                    temp.Add("0");

                }
                equipo.Add(temp);
            }
            ViewBag.Equipo_Solict = equipo;
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
                             where o.ID == id && o2.CANTIDAD > 0
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };


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

                        if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add(""); }
                        equipo.Add(temp);
                    }
                }
            }
            var prestamos = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == id);
            DateTime dt = prestamos.FECHA_RETIRO.Value;
            dt = dt.AddDays(prestamos.PERIODO_USO);
            var equip = prestamos.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);
            var fechas = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).Where(p => p.FECHA_RETIRO <= prestamos.FECHA_RETIRO && p.ID != id);
            Dictionary<string, int> eq = new Dictionary<string, int>();
            foreach (var f in fechas)
            {
                if (f.FECHA_RETIRO.Value.AddDays(f.PERIODO_USO) >= prestamos.FECHA_RETIRO.Value)
                {
                    var equip2 = f.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);
                    foreach (var e in equip2)
                    {
                        foreach (var pp in equip)
                        {
                            if (e.TIPO_ACTIVO == pp.TIPO_ACTIVO)
                            {
                                if (eq.ContainsKey(pp.TIPO_ACTIVO.ToString()))
                                {
                                    int value = Convert.ToInt32(eq[pp.TIPO_ACTIVO.ToString()]);
                                    value += Convert.ToInt32(e.CANTIDAD);
                                    eq[pp.TIPO_ACTIVO.ToString()] = value;
                                }
                                else
                                {
                                    eq.Add(pp.TIPO_ACTIVO.ToString(), int.Parse(e.CANTIDAD.ToString()));
                                }
                            }
                        }
                    }
                }
            }

            List<string> disp = new List<string>();
            foreach (var e in equip)
            {
                int tipo = Convert.ToInt32(e.TIPO_ACTIVO);
                int contador = (from a in db.ACTIVOS
                                where a.TIPO_ACTIVOID == tipo
                                select a).Count();
                int total = 0;
                if (eq.ContainsKey(e.TIPO_ACTIVO))
                {

                    total = Convert.ToInt32(e.CANTIDAD) + eq[e.TIPO_ACTIVO];
                }
                else
                {
                    total = Convert.ToInt32(e.CANTIDAD);
                }
                if (total <= contador)
                {
                    disp.Add("d");
                }
                else
                {
                    disp.Add("i");
                }
            }
            int k = 0;
            foreach (var l in equipo)
            {
                l.Add(disp[k]);
                k++;
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
        [HttpPost]
        public ActionResult Details(string ID, int[] cantidad_aprobada, string b)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(ID);

            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where o.ID == ID && o2.CANTIDAD > 0
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };


            if (b == "Aceptar")
            {

                int a = 0;
                foreach (var x in equipo_sol)
                {
                    if (x.ID == ID)
                    {
                        if (x.ID == x.ID_EQUIPO)
                        {

                            EQUIPO_SOLICITADO P = db.EQUIPO_SOLICITADO.Find(ID, x.TIPO, x.CANTIDAD);

                            decimal temp = cantidad_aprobada[a];

                            P.CANTIDADAPROBADA = temp;

                            if (ModelState.IsValid)
                            {
                                db.Entry(P).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            a++;
                        }
                    }
                }

                pRESTAMO.Estado = 2;
                if (ModelState.IsValid)
                {
                    db.Entry(pRESTAMO).State = EntityState.Modified;
                    db.SaveChanges();
                }

                ViewBag.Mensaje = "El préstamo ha sido aprobado con éxito";

            }

            if (b == "Denegar")
            {

                pRESTAMO.Estado = 3;
                if (ModelState.IsValid)
                {
                    db.Entry(pRESTAMO).State = EntityState.Modified;
                    db.SaveChanges();
                }

                ViewBag.Mensaje2 = "El préstamo ha sido denegado con éxito";
            }



            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.ID == ID
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };

            foreach (var m in lista)
            {
                if (m.Prestamo.ID == ID)
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



            var equipo = new List<List<String>>();
            foreach (var x in equipo_sol)
            {
                if (x.ID == ID)
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

                        if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add(""); }
                        equipo.Add(temp);
                    }
                }
            }


            var prestamos = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == ID);
            DateTime dt = prestamos.FECHA_RETIRO.Value;
            dt = dt.AddDays(prestamos.PERIODO_USO);

            var equip = prestamos.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);
            var fechas = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).Where(p => p.FECHA_RETIRO <= prestamos.FECHA_RETIRO && p.ID != ID);
            Dictionary<string, int> eq = new Dictionary<string, int>();
            foreach (var f in fechas)
            {
                if (f.FECHA_RETIRO.Value.AddDays(f.PERIODO_USO) >= prestamos.FECHA_RETIRO.Value)
                {
                    var equip2 = f.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);
                    foreach (var e in equip2)
                    {
                        foreach (var pp in equip)
                        {
                            if (e.TIPO_ACTIVO == pp.TIPO_ACTIVO)
                            {
                                if (eq.ContainsKey(pp.TIPO_ACTIVO.ToString()))
                                {
                                    int value = Convert.ToInt32(eq[pp.TIPO_ACTIVO.ToString()]);
                                    value += Convert.ToInt32(e.CANTIDAD);
                                    eq[pp.TIPO_ACTIVO.ToString()] = value;
                                }
                                else
                                {
                                    eq.Add(pp.TIPO_ACTIVO.ToString(), int.Parse(e.CANTIDAD.ToString()));
                                }
                            }
                        }
                    }
                }
            }

            List<string> disp = new List<string>();
            foreach (var e in equip)
            {
                int tipo = Convert.ToInt32(e.TIPO_ACTIVO);
                int contador = (from a in db.ACTIVOS
                                where a.TIPO_ACTIVOID == tipo
                                select a).Count();
                int total = 0;
                if (eq.ContainsKey(e.TIPO_ACTIVO))
                {

                    total = Convert.ToInt32(e.CANTIDAD) + eq[e.TIPO_ACTIVO];
                }
                else
                {
                    total = Convert.ToInt32(e.CANTIDAD);
                }
                if (total <= contador)
                {
                    disp.Add("d");
                }
                else
                {
                    disp.Add("i");
                }
            }
            int k = 0;
            foreach (var l in equipo)
            {
                l.Add(disp[k]);
                k++;
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

        /*@if (ViewBag.Disponible)
        {

        }
        else
        {
            <td>

                @Html.TextBox("cantidad_apovada", ViewBag.CurrentFilter as List<decimal>, new { style = "color:#1e83ca;", @class = "warning form-control col-md-2", @type = "number", @min = "0", @value = "0", @max = "99" })
            </td>
        }

            */

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
            TempData.Keep();

            return View();
        }

        public long calcularNumBoleta()
        {
            var prestamos = from s in db.PRESTAMOS
                            select s;
            prestamos = prestamos.OrderBy(s => s.NUMERO_BOLETA);
            
            List<PRESTAMO> u = prestamos.ToList();
            PRESTAMO ultimo = u.First<PRESTAMO>();

            return ultimo.NUMERO_BOLETA.GetValueOrDefault() + 1;
        }
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,SIGLA_CURSO,Estado,CED_SOLICITA,CED_APRUEBA")] PRESTAMO p, int[] Cantidad, String[] Categoria)
        {
            //p.FECHA_RETIRO
            PRESTAMO prestamo = new PRESTAMO();
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                prestamo.ID = generarID();
                prestamo.MOTIVO = p.MOTIVO;
                prestamo.NUMERO_BOLETA = calcularNumBoleta();
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
                List<String> cat = (List<String>) TempData["categorias"];
                for (int i = 0; i < Cantidad.Length; i++)
                {
                    EQUIPO_SOLICITADO equipo = new EQUIPO_SOLICITADO();
                    if (Cantidad[i] == 0)
                    {
                        equipo.CANTIDAD = 0;
                    } else
                    {
                        equipo.CANTIDAD = Cantidad[i];
                    }
                    equipo.TIPO_ACTIVO = traerCategoria( cat[i] );
                    equipo.ID_PRESTAMO = prestamo.ID;
                    db.EQUIPO_SOLICITADO.Add(equipo);
                    db.SaveChanges();
                }
                TempData["confirmacion"] = "La solicitud fue enviada con éxito";
                TempData.Keep();
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

            /*  -------------------------------------------------------------------------------------------  */
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID) 
                       select new { t.NOMBRE, t.ID }).Distinct();

            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id )
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };

            var equipo = new List<List<String>>();
            cat = cat.OrderBy(t => t.NOMBRE);
            foreach (var y in cat)
            {
                bool existeCategoria=false;
                List<String> temp = new List<String>();
                foreach (var x in equipo_sol)
                {
                    
                    if (x.TIPO != null)
                    {
                        if (x.TIPO == y.ID.ToString())
                        {

                            temp.Add(y.NOMBRE);
                            existeCategoria = true;
                            if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add("0"); }
                            if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add("0"); }
                            break;
                        }
                    }
                    else
                    {
                        temp.Add("");
                        temp.Add("");
                        temp.Add("");
                    }
                }
                if (!existeCategoria)
                {
                    temp.Add(y.NOMBRE);
                    temp.Add("0");
                    temp.Add("0");
                    
                }
                equipo.Add(temp);
            }
            ViewBag.Equipo_Solict = equipo;
            return View(pRESTAMO);
        }

        // POST: PRESTAMOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO p, string id, int[] cantidad, string b)
        {
            PRESTAMO P = db.PRESTAMOS.Find(id);
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();
            cat = cat.OrderBy(t => t.NOMBRE);
            if (b == "Aceptar")
            {
                int a = 0;
                foreach (var y in cat)
                {
                    bool noEsta = true;
                    foreach (var x in equipo_sol)
                    {
                        /*if (x.ID == id)
                        {
                            if (x.ID == x.ID_EQUIPO)
                            {*/
                        if (y.NOMBRE == x.TIPO)
                        {
                            EQUIPO_SOLICITADO pr = db.EQUIPO_SOLICITADO.Find(id, x.TIPO, x.CANTIDAD);
                            if (pr == null)
                            {
                                pr = new EQUIPO_SOLICITADO();
                                pr.ID_PRESTAMO = id;
                                pr.TIPO_ACTIVO = y.NOMBRE;//traerCategoria(cat[a]); ;
                                pr.CANTIDAD = cantidad[a];
                                if (ModelState.IsValid)
                                {
                                    db.EQUIPO_SOLICITADO.Add(pr);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                decimal temp = cantidad[a];
                                noEsta = false;
                                pr.CANTIDAD = temp;
                                noEsta = false;
                                if (ModelState.IsValid)
                                {
                                    db.Entry(pr).State = EntityState.Modified;
                                    db.SaveChanges();
                                    db.SaveChanges();
                                }
                            }

                        }
                        /* }
                     }*/
                    }
                    a++;
                    if (noEsta)
                    {
                        EQUIPO_SOLICITADO pr = new EQUIPO_SOLICITADO();
                        pr.ID_PRESTAMO = id;
                        pr.TIPO_ACTIVO = y.NOMBRE;//traerCategoria(cat[a]); ;
                        pr.CANTIDAD = cantidad[a];
                        if (ModelState.IsValid)
                        {
                            db.EQUIPO_SOLICITADO.Add(pr);
                            db.SaveChanges();
                        }
                        /*EQUIPO_SOLICITADO pr = new EQUIPO_SOLICITADO();
                        pr.ID_PRESTAMO = id;
                        pr.TIPO_ACTIVO = y.ID.ToString();
                        pr.CANTIDAD = cantidad[a];
                        pr.CANTIDADAPROBADA = 0;
                        if (ModelState.IsValid)
                        {
                            db.EQUIPO_SOLICITADO.Add(pr);
                            db.SaveChanges();
                        }*/
                    }

                }
                ViewBag.Mensaje = "El préstamo ha sido aprobado con éxito";

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

            /*
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id )
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();
            cat = cat.OrderBy(t => t.NOMBRE);
            if (b == "Aceptar")
            {
                int a = 0;
                foreach (var y in cat)
                {
                    bool noEsta = true;
                    foreach (var x in equipo_sol)
                    {
                        if (x.ID == id)
                        {
                            if (x.ID == x.ID_EQUIPO)
                            {

                                EQUIPO_SOLICITADO pr = db.EQUIPO_SOLICITADO.Find(id, x.TIPO, x.CANTIDAD);
                                if (pr == null)
                                {
                                    pr = new EQUIPO_SOLICITADO();
                                    pr.ID_PRESTAMO = id;
                                    pr.TIPO_ACTIVO = y.NOMBRE;//traerCategoria(cat[a]); ;
                                    pr.CANTIDAD = cantidad[a];
                                    if (ModelState.IsValid)
                                    {
                                        db.EQUIPO_SOLICITADO.Add(pr);
                                        db.SaveChanges();
                                    }
                                }
                                else 
                                {
                                    decimal temp = cantidad[a];

                                    pr.CANTIDAD = temp;
                                    noEsta = false;
                                    if (ModelState.IsValid)
                                    {
                                        db.Entry(pr).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }


                                a++;
                            }
                        }
                    }
                    if (!noEsta)
                    {
                        EQUIPO_SOLICITADO pr = new EQUIPO_SOLICITADO();
                        pr.ID_PRESTAMO = id;
                        pr.TIPO_ACTIVO = y.NOMBRE;//traerCategoria(cat[a]); ;
                        pr.CANTIDAD = cantidad[a];
                        if (ModelState.IsValid)
                        {
                            db.EQUIPO_SOLICITADO.Add(pr);
                            db.SaveChanges();
                        }
                        /*EQUIPO_SOLICITADO pr = new EQUIPO_SOLICITADO();
                        pr.ID_PRESTAMO = id;
                        pr.TIPO_ACTIVO = y.ID.ToString();
                        pr.CANTIDAD = cantidad[a];
                        pr.CANTIDADAPROBADA = 0;
                        if (ModelState.IsValid)
                        {
                            db.EQUIPO_SOLICITADO.Add(pr);
                            db.SaveChanges();
                        }* /
                    }

                }
                ViewBag.Mensaje = "El préstamo ha sido aprobado con éxito";

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
          */
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

            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();

            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            /*
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

                        if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add("0"); }
                        if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add("0"); }
                        equipo.Add(temp);
                    }
                }
            }
            ViewBag.Equipo_Solict = equipo;
            */
            var equipo = new List<List<String>>();

            foreach (var y in cat)
            {
                bool existeCategoria = false;
                List<String> temp = new List<String>();
                foreach (var x in equipo_sol)
                {

                    if (x.TIPO != null)
                    {
                        if (x.TIPO == y.ID.ToString())
                        {

                            temp.Add(y.NOMBRE);
                            existeCategoria = true;
                            if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add("0"); }
                            if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add("0"); }
                            break;
                        }
                    }
                    else
                    {
                        temp.Add("");
                        temp.Add("");
                        temp.Add("");
                    }
                }
                if (!existeCategoria)
                {
                    temp.Add(y.NOMBRE);
                    temp.Add("0");
                    temp.Add("0");

                }
                equipo.Add(temp);
            }
            ViewBag.Equipo_Solict = equipo;
            return View(pRESTAMO);
        }
        // POST: PRESTAMOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            pRESTAMO.Estado = 6;
            if (ModelState.IsValid)
            {
                db.Entry(pRESTAMO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Historial");
            }
            return RedirectToAction("Historial");
        }
        /*  // POST: PRESTAMOes/Delete/5
          [HttpPost, ActionName("Delete")]
          [ValidateAntiForgeryToken]
          public ActionResult DeleteConfirmed(string id)
          {
              PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
              pRESTAMO.Estado = 6;
              db.SaveChanges();
              return RedirectToAction("Historial");
          }*/

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
            Aprobada,
            Denegada,
            Abierta,
            Cerrada,
            Cancelada
        }
    }
}
