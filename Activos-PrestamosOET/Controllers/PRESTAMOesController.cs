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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
//using System.Security.Claims;




//using System.Security.Claims;
//using System.Threading.Tasks;

//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using Microsoft.Owin.Security;


using SendGrid;

using System.Configuration;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
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

        protected List<List<String>> equipoPorCategoria(int cat, String id)
        {
            List<List<String>> equipos = new List<List<String>>();

            var activos = db.PRESTAMOS.Include(i => i.ACTIVOes).SingleOrDefault(h => h.ID == id);
            // int cat = int.Parse(categoria);
            var act = from a in activos.ACTIVOes.Where(i => i.TIPO_ACTIVOID == cat)
                      select new { FABRICANTE = a.FABRICANTE, MODELO = a.MODELO, PLACA = a.PLACA };

            foreach (var a in act)
            {
                List<String> equipo = new List<String>();
                equipo.Add(a.FABRICANTE);
                equipo.Add(a.MODELO);
                equipo.Add(a.PLACA);
                equipos.Add(equipo);
            }

            return equipos;
        }


        protected int traerCategoria(String tipo)
        {
            var consultaCat = from t in db.TIPOS_ACTIVOS
                              where t.NOMBRE.Equals(tipo)
                              select t.ID;

            List<String> categorias = new List<String>();

            foreach (int c in consultaCat)
            {
                categorias.Add(c.ToString());
            }
            int cat = Int32.Parse(categorias[0]);

            return cat;
        }

        // GET: PRESTAMOes
        //Requiere: Recibe 6 parámetros, el primero es la columna por la que se ordenan los datos en la tabla, el segundo, tercero, cuarto y quinto para hacer filtrado de búsqueda y el último para identificar la página en q se encuentra la tabla.
        // Modifica: Maneja el index view, la cual es la vista de consulta de revisión de solicitudes.
        //Retorna: Devuelve una tabla que se despliegará en el index de Revisión de solicitudes.
        public ActionResult Index(string sortOrder, string currentFilter, string fechaSolicitud, string fechaRetiro, string estado, int? page)
        {
            //se identifica si alguna columna fue seleccionada como filtro para ordenar los datos despliegados
            ViewBag.currentSort = sortOrder;
            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "numero_dsc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
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
            //var prestamos = db.PRESTAMOS.Include(i => i.USUARIO);//Se agrega la tabla de usuarios a la de préstamos

            //Inician filtros de búsqueda
            if (!String.IsNullOrEmpty(fechaSolicitud) || !String.IsNullOrEmpty(fechaRetiro))//Caso en que se consulta por una fecha específica
            {
                DateTime fechaS;
                DateTime fechaR;


                if (String.IsNullOrEmpty(fechaSolicitud))//Se ingresó únicamente fecha de inicio del préstamo
                {
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Day == fechaR.Day);
                    }
                }
                else if (String.IsNullOrEmpty(fechaRetiro))//Se ingresó únicamente la fecha de solicitud del préstamo
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Year == fechaS.Year
                                                          && model.FECHA_SOLICITUD.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Day == fechaS.Day);
                    }
                }
                else//Se ingresaron tanto la fecha de solicitud como de inicio del préstamo.
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
            if (!string.IsNullOrEmpty(estado) && estado != "0")//Se consulta por un estado específico. Cero significa todos.
            {
                int est = int.Parse(estado);
                var int16 = Convert.ToInt16(est);
                prestamos = prestamos.Where(model => model.Estado == int16);
            }
            //Finaliza búsqueda por filtros//

            //Inicio ordenado por columnas//
            switch (sortOrder)//Se ordena la tabla por una columna seleccionada en la vista.
            {
                case "numero_dsc"://Se ordena descendentemente por número de boleta
                    prestamos = prestamos.OrderByDescending(s => s.NUMERO_BOLETA);
                    break;
                case "Date"://Se ordena ascendentemente por fecha de solicitud
                    prestamos = prestamos.OrderBy(s => s.FECHA_SOLICITUD);
                    break;
                case "date_desc"://Se ordena descendentemente por fecha de solicitud
                    prestamos = prestamos.OrderByDescending(s => s.FECHA_SOLICITUD);
                    break;
                case "FDate"://Se ordena ascendentemente por fecha de inicio del préstamo
                    prestamos = prestamos.OrderBy(s => s.FECHA_RETIRO);
                    break;
                case "FDate_desc"://Se ordena descendentemente por fecha de inicio del préstamo
                    prestamos = prestamos.OrderByDescending(s => s.FECHA_RETIRO);
                    break;
                /*case "Periodo":
                    var press = prestamos.ToList().OrderBy(s => s.FECHA_RETIRO.Value.AddDays(s.PERIODO_USO));
                    //prestamos = from pp in prestamos
                    //       orderby (DbFunctions.AddDays(pp.FECHA_RETIRO, pp.PERIODO_USO)) select pp;  
                    // prestamos = prestamos.Cast(press);
                    prestamos = prestamos.OrderBy(s => s.PERIODO_USO).OrderBy(k => k.FECHA_RETIRO.Value.AddDays(k.PERIODO_USO));
                    break;
                case "Periodo_desc":
                    prestamos = prestamos.OrderByDescending(s => s.FECHA_RETIRO.Value.AddDays(s.PERIODO_USO));
                    break;*/
                case "Name"://Ordenado ascendentemento por nombre del solicitante
                    prestamos = prestamos.OrderBy(s => s.CED_SOLICITA);
                    break;
                case "Name_desc"://Ordenado descendentemento por nombre de solicitante
                    prestamos = prestamos.OrderByDescending(s => s.CED_SOLICITA);
                    break;
                default:  // Para todo otro caso se ordena ascendentemente por número de boleta
                    prestamos = prestamos.OrderBy(s => s.NUMERO_BOLETA);
                    break;
            }
            //Finaliza ordenado por columnas//

            //Inicia paginación//
            int pageSize = 5;//Se define número filar por página a desplegar
            int pageNumber = (page ?? 1);//Se define el número de página en que se encuentra
            return View(prestamos.ToPagedList(pageNumber, pageSize));//Se envía la tabla a la paginación
            //Finaliza paginación//                            
        }

        // GET: PRESTAMOes/Historial
        //Requiere: cédula del solicitante, filtro actual de categorías, hilera del estado de la revisión y el identificador de la página en la que se encuentra actualmente.
        //Modifica: Carga la información de la tabla con el historial de solicitudes.
        //Retorna: vista con la tabla en la que se despliega el historial de solicitudes.
        public ActionResult Historial(string CED_SOLICITA, string currentFilter, string estado, int? page)
        {
            //CED_SOLICITA = "PITAN0126052014.085230671";
            //Para que al refrescar la pagina no se quite el filtro por estado
            ViewBag.estado = estado;

            ViewBag.mensajeConfirmacion = (String)TempData["confirmacion"];
            //Consulta todos los prestamos
            var prestamos = from s in db.PRESTAMOS select s;
            //Este es el filtro por cedula del que solicita. Esto deberia ser automatico y filtrar las solicitudes 
            //por el usuario loggeado pero como aun no esta la parte del log in el filtro lo pondremos en 
            //este if por mientras
            if (!string.IsNullOrEmpty(CED_SOLICITA))
            {
                prestamos = prestamos.Where(model => model.CED_SOLICITA == CED_SOLICITA);
            }
            //Verfica el filtro de estado. Si el usuario no selecciono ningun filtro, entonces no se filtra por estado
            //pero si si selecciono el estado por el que quiere filtrar entonces, filtra por eso
            int est;
            if (string.IsNullOrEmpty(estado))
            {
                est = 0;
            }
            else
            {
                est = int.Parse(estado);
            }
            if (!string.IsNullOrEmpty(estado) && estado != "0")
            {
                var int16 = Convert.ToInt16(est);
                prestamos = prestamos.Where(model => model.Estado == int16);
            }
            //En el historial, las solicitudes siempre estan ordenadas por la fecha de solicitud (de la mas reciente a la mas vieja)
            prestamos = prestamos.OrderByDescending(s => s.FECHA_SOLICITUD);
            //para la paginacion de la tabla
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(prestamos.ToPagedList(pageNumber, pageSize));
        }

        public string viewBagFechaSolicitada(DateTime sol)
        {
            string f = sol.Date.ToShortDateString();
            f = f.Replace("/20", "/");
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
            ViewBag.Estadillo = "";
            if (pRESTAMO.Estado == 1)
            {
                ViewBag.Estadillo = "Pendiente";
            }
            else if (pRESTAMO.Estado == 2)
            {
                ViewBag.Estadillo = "Aceptada";
            }
            else if (pRESTAMO.Estado == 3)
            {
                ViewBag.Estadillo = "Denegada";
            }
            else if (pRESTAMO.Estado == 4)
            {
                ViewBag.Estadillo = "Abierta";
            }
            else if (pRESTAMO.Estado == 5)
            {
                ViewBag.Estadillo = "Cerrada";
            }
            else if (pRESTAMO.Estado == 6)
            {
                ViewBag.Estadillo = "Cancelada";
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
                        if (x.TIPO == y.NOMBRE)
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
        //Requiere: Recibe el id del prestamo que se está consultando.
        // Modifica: Maneja el details view, la cual es la vista de consulta de revisión de una solicitud en particular.
        //Retorna: Devuelve un información necesaria para el despliegue de la vista como: nombre de solicitante, el estado, el equipo solicitado y sus cantidades
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["Mensaje"] != null)
            {
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
                TempData.Remove("Mensaje");
            }
            if (TempData["Mensaje2"] != null)
            {
                ViewBag.Mensaje2 = TempData["Mensaje2"].ToString();
                TempData.Remove("Mensaje2");
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
            
            
            var equipoSol = db.PRESTAMOS.Include(i => i.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == id);
            var equipoSolicitado = equipoSol.EQUIPO_SOLICITADO;

            var equipo = new List<List<String>>();
            var actPrevios = new List<List<String>>();
            var act = new List<List<String>>();
            var activos = new List<List<List<String>>>();
            var activosPrevios = new List<List<List<String>>>();
            foreach (var x in equipoSolicitado)
            {                
                List<String> temp = new List<String>();
                if (x.TIPO_ACTIVO != null)
                {   
                                             
                    temp.Add(x.TIPO_ACTIVO.ToString());
                    actPrevios = llenarTablaDetails(x.TIPOS_ACTIVOSID.ToString(), id);
                    act = llenarTablaDetails(x.TIPOS_ACTIVOSID.ToString());
                }
                else
                {
                    temp.Add("");
                }
                if (x.CANTIDAD != 0)
                {
                    temp.Add(x.CANTIDAD.ToString());
                }
                else
                {
                    temp.Add("");
                }
                if (x.CANTIDADAPROBADA != 0)
                {
                    temp.Add(x.CANTIDADAPROBADA.ToString());
                }
                else
                {
                    temp.Add("");
                }
                equipo.Add(temp);
                activosPrevios.Add(actPrevios);
                activos.Add(act);
            }
            ViewBag.Activos_enPrevio = activosPrevios;
            ViewBag.Activos_enCat = activos;
        //Segmento de código para colocar colores a las cantidad de solicitudes por categoría.
        var prestamosConEquipo = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == id);//Se hace joint entre prestamos y equipo solicitado por id del préstamo           
            var equipoMayorCero = prestamosConEquipo.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);//Se verifica que se seleccionen los equipos seleccionados que tengan más 0 soliciudes
            var prestamosPorFechas = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).Where(p => p.FECHA_RETIRO <= prestamosConEquipo.FECHA_RETIRO && p.ID != id);//Se selccionan las solicitudes de préstamo qe se encuentran "abiertos" para el momento de inicio del préstamo consultado.
            Dictionary<string, int> hashConValoresPorTipoActivo = new Dictionary<string, int>();//diccionario que almacena las cantidades de préstamos vigentes.
            if (prestamosPorFechas.ToList() != null)
            {
                foreach (var f in prestamosPorFechas)
                {
                    if (f.FECHA_RETIRO.AddDays(f.PERIODO_USO) >= prestamosConEquipo.FECHA_RETIRO)
                    {
                        var equipoFechasMayorCero = f.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);//Se seleccionan pedidos con una cantidad mayor a 0
                        if (equipoFechasMayorCero.ToList() != null)
                        {
                            foreach (var e in equipoFechasMayorCero)
                            {
                                foreach (var pp in equipoMayorCero)
                                {
                                    if (e.TIPO_ACTIVO == pp.TIPO_ACTIVO)//Si los prestamos "abiertos" tienen un articulo solicitado en el prestamo consultado, se registra la categoría y la cantidad
                                    {//Se guarda en el diccionario
                                        if (hashConValoresPorTipoActivo.ContainsKey(pp.TIPOS_ACTIVOSID.ToString()))
                                        {
                                            int value = Convert.ToInt32(hashConValoresPorTipoActivo[pp.TIPOS_ACTIVOSID.ToString()]);
                                            value += Convert.ToInt32(e.CANTIDAD);
                                            hashConValoresPorTipoActivo[pp.TIPOS_ACTIVOSID.ToString()] = value;
                                        }
                                        else
                                        {
                                            hashConValoresPorTipoActivo.Add(pp.TIPOS_ACTIVOSID.ToString(), int.Parse(e.CANTIDAD.ToString()));
                                        }
                                    }
                                }
                            }
                        }                       
                    }
                }
            }

            List<string> disp = new List<string>();
            foreach (var e in equipoMayorCero)
            {
                
                int tipo = e.TIPOS_ACTIVOSID;
                //Se consulta la cantidad total de activos prestables de una categoría 
                int contador = (from a in db.ACTIVOS
                                where a.TIPO_ACTIVOID == tipo
                                select a).Count();
                int total = 0;
                if (hashConValoresPorTipoActivo.ContainsKey(e.TIPOS_ACTIVOSID.ToString()))
                {

                    total = Convert.ToInt32(e.CANTIDAD) + hashConValoresPorTipoActivo[e.TIPOS_ACTIVOSID.ToString()];//
                }
                else
                {
                    total = Convert.ToInt32(e.CANTIDAD);
                }
                if (total <= contador)//Si el total de activos solicitados para un periodo específico es meonr que el total de activos prestables, se retorna un disponible ("d")
                {
                    disp.Add("d");
                }
                else//Caso contrario, se retorna un indisponible ("i")
                {
                    disp.Add("i");
                }
            }
            int k = 0;
            foreach (var l in equipo)//Se agrega el resultado del cálculo para cada categoría al final del vector a retornar.
            {
                l.Add(disp[k]);
                k++;
            }
            ViewBag.Equipo_Solict = equipo;

            
            return View(pRESTAMO);
        }


        //Requiere: Recibe el id del prestamo que se está consultando, un vector desde la vista que envia las cantidades de equipo solicitado y un string q se usa para determinar que boton fue apretado.
        // Modifica: Maneja el details view, la cual es la vista de consulta de revisión de una solicitud en particular.
        //Retorna: Devuelve un información necesaria para el despliegue de la vista como: nombre de solicitante, el estado, el equipo solicitado y sus cantidades, además, despliega un mensaje de confirmacion diferente de acuerdo a si el boton fue aceptar o denegar

        [HttpPost]
        public ActionResult Details(string ID, int[] cantidad_aprobada, string[] activoSeleccionado, string b, [Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO p)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(ID);
            pRESTAMO.OBSERVACIONES_APROBADO = p.OBSERVACIONES_APROBADO;
            if (ModelState.IsValid)
            {
                db.Entry(pRESTAMO).State = EntityState.Modified;
                db.SaveChanges();
            }

            var prestamo = db.PRESTAMOS.Include(i => i.EQUIPO_SOLICITADO).SingleOrDefault(h => h.ID == ID);

            var equipo_sol = prestamo.EQUIPO_SOLICITADO;

            if (b == "Aceptar")
            {
                int a = 0;
                foreach (var x in equipo_sol)
                {
                    if (prestamo.ID == x.ID_PRESTAMO)
                    {

                        EQUIPO_SOLICITADO P = db.EQUIPO_SOLICITADO.Find(ID, x.TIPO_ACTIVO, x.CANTIDAD);

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

                if (pRESTAMO.Estado == 1)
                {
                    pRESTAMO.Estado = 2;
                    if (ModelState.IsValid)
                    {
                        db.Entry(pRESTAMO).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                addActivosToPrestamo(activoSeleccionado, ID);
                ViewBag.Mensaje = "El préstamo ha sido aprobado con éxito";
                TempData["Mensaje"] = "El préstamo ha sido aprobado con éxito";

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
                TempData["Mensaje2"] = "El préstamo ha sido denegado con éxito";
            }

            if (b == "Descargar Boleta")
            {
                DownloadPDF("BoletaPDF", pRESTAMO, "BoletaSoliciud");
            }

            return RedirectToAction("Details", new { id = ID });
        }
        private static void SendAsync(SendGrid.SendGridMessage message)
        {

            //string apikey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            // Create a Web transport for sending email.
            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["mailAccount"],
                       ConfigurationManager.AppSettings["mailPassword"]
                       );
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(message);
        }

        private static void SolicitudBien(string to, string mensaje,string subj)
        {
            // Create the email object first, then add the properties.
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(to);
            myMessage.From = new System.Net.Mail.MailAddress(
                                "andresbejar@gmail.com", "Admin");
            myMessage.Subject = subj; 
            myMessage.Text = mensaje;
            myMessage.Html = mensaje;

            SendAsync(myMessage);
        }

        // GET: PRESTAMOes/Create
        public ActionResult Create()
        {

            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1");
            ViewBag.Cursos = new SelectList(db.V_COURSES, "COURSES_CODE", "COURSE_NAME");

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
                string idd = generarID();
                string cedSol = p.CED_SOLICITA;
                prestamo.ID = idd;
                prestamo.MOTIVO = p.MOTIVO;
                prestamo.NUMERO_BOLETA = 1;// calcularNumBoleta();
                prestamo.OBSERVACIONES_APROBADO = "";
                prestamo.OBSERVACIONES_RECIBIDO = "";
                prestamo.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
                prestamo.PERIODO_USO = p.PERIODO_USO;
                prestamo.SIGLA_CURSO = p.SIGLA_CURSO;
                prestamo.CED_APRUEBA = p.CED_APRUEBA;
                prestamo.CED_SOLICITA = cedSol;
                prestamo.FECHA_RETIRO = p.FECHA_RETIRO;
                prestamo.FECHA_SOLICITUD = System.DateTimeOffset.Now.Date;//SELECT SYSDATE FROM DUAL
                prestamo.SOFTWARE_REQUERIDO = p.SOFTWARE_REQUERIDO;
                prestamo.Estado = 1;
                db.PRESTAMOS.Add(prestamo);
                db.SaveChanges();
                List<String> cat = (List<String>)TempData["categorias"];
                for (int i = 0; i < Cantidad.Length; i++)
                {
                    EQUIPO_SOLICITADO equipo = new EQUIPO_SOLICITADO();
                    if (Cantidad[i] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        equipo.CANTIDAD = Cantidad[i];
                    }
                    equipo.TIPO_ACTIVO = cat[i];
                    equipo.TIPOS_ACTIVOSID = traerCategoria(cat[i]);
                    equipo.ID_PRESTAMO = prestamo.ID;
                    db.EQUIPO_SOLICITADO.Add(equipo);
                    db.SaveChanges();
                }
                PRESTAMO prest =  new PRESTAMO();
                prest=db.PRESTAMOS.Find(idd);
                //Refresca el contexti del objeto PRESTAMO prest de la base de datos para obtener el numero de solicitud correcto.
                var ctx = ((IObjectContextAdapter)db).ObjectContext;
                ctx.Refresh(RefreshMode.ClientWins, prest);

                //Envia el correo de notificacion
                string subj = "Solicitud de Prestamo: "+prest.NUMERO_BOLETA.ToString();
                var consultaUrl = Url.Action("Detalles", "PRESTAMOes", new { id = idd }, protocol: Request.Url.Scheme);
                string link = " " + consultaUrl + " ";
                string mensajito = "Su solicitud ha sido realizada con éxito." +" \n "+ "El numero de boleta es " + prest.NUMERO_BOLETA.ToString() + ". \n " + " Puedes consultar la solicitud en el siguiente link:"+link;
                USUARIO este = db.USUARIOS.Find(cedSol);
                string email = User.Identity.Name; 
                SolicitudBien(email, mensajito, subj);
                TempData["confirmacion"] = "La solicitud fue enviada con éxito";
                TempData.Keep();
                return RedirectToAction("Historial");
            }
            
            ViewBag.CED_SOLICITA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_SOLICITA);
            ViewBag.CED_APRUEBA = new SelectList(db.USUARIOS, "IDUSUARIO", "USUARIO1", p.CED_APRUEBA);
            return View(prestamo);
        }


        // GET: PRESTAMOes/Edit/5
        //Requiere: identificador del Préstamo.
        //Modifica: Carga los campos en los que se pueden cambiar datos para editar información relacionada a un préstamo específico.
        //Retorna: vista con los campos para editar solicitud.
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Busca la solicitud que se quiere editar
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            //si no esta devuelve error
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cursos = new SelectList(db.V_COURSES, "COURSES_CODE", "COURSE_NAME");
            //Determina el estado de la solicitud para desplegarlo en la pantalla mas adelante
            ViewBag.Estadillo = "";
            if (pRESTAMO.Estado == 1)
            {
                ViewBag.Estadillo = "Pendiente";
            }
            else if (pRESTAMO.Estado == 2)
            {
                ViewBag.Estadillo = "Aceptada";
            }
            else if (pRESTAMO.Estado == 3)
            {
                ViewBag.Estadillo = "Denegada";
            }
            else if (pRESTAMO.Estado == 4)
            {
                ViewBag.Estadillo = "Abierta";
            }
            else if (pRESTAMO.Estado == 5)
            {
                ViewBag.Estadillo = "Cerrada";
            }
            else if (pRESTAMO.Estado == 6)
            {
                ViewBag.Estadillo = "Cancelada";
            }
            ViewBag.fechSol = viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD.Date);
            //Consulta el usuario solicitante
            var lista = from o in db.PRESTAMOS
                        from o2 in db.USUARIOS
                        where o.ID == id
                        select new { Prestamo = o, CEDULA = o2.IDUSUARIO, USUARIO = o2.NOMBRE };
            //busca el nombre del usuario solicitante
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

            //Busca las categorias existentes
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();
            //busca al equipo previamente solicitado 
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            //Acomoda la informacion de la tabla de equipo solicitado que se desplegara en la pantalla
            var equipo = new List<List<String>>();
            cat = cat.OrderBy(t => t.NOMBRE);
            foreach (var y in cat)
            {
                bool existeCategoria = false;
                List<String> temp = new List<String>();
                foreach (var x in equipo_sol)
                {

                    if (x.TIPO != null)
                    {
                        if (x.TIPO == y.NOMBRE)
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
            //envia la informacion del equipo solicitado a modo de ViewBag
            ViewBag.Equipo_Solict = equipo;
            return View(pRESTAMO);
        }


        //Requiere: Un objeto prestamo, id del prestamo a modificar, int[] cantidad que dice las cantidades de las categorias de ahora.
        //Modifica: Actualiza en la base de datos la informacion relacionada con ese prestamo.
        //Retorna: vista con los campos para editar solicitud.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NUMERO_BOLETA,MOTIVO,FECHA_SOLICITUD,FECHA_RETIRO,PERIODO_USO,SOFTWARE_REQUERIDO,OBSERVACIONES_SOLICITANTE,OBSERVACIONES_APROBADO,OBSERVACIONES_RECIBIDO,CEDULA_USUARIO,SIGLA_CURSO")] PRESTAMO p, string id, int[] cantidad, string b)
        {
            //Busca el prestamo en la base de datos
            PRESTAMO P = db.PRESTAMOS.Find(id);
            string numBol = P.NUMERO_BOLETA.ToString();
            //Busca el equipo previamente solicitado
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            //Determina las categorias de activos presentes en el sistema
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) && t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();
            cat = cat.OrderBy(t => t.NOMBRE);
            int a = 0;
            //Para guardar cambios en la tabla de equipo solicitado
            foreach (var y in cat)
            {
                bool noEsta = true;
                foreach (var x in equipo_sol)
                {
                    if (y.NOMBRE == x.TIPO)
                    {
                        EQUIPO_SOLICITADO pr = db.EQUIPO_SOLICITADO.Find(id, y.NOMBRE, x.CANTIDAD);
                        //busca si el elemento de la tabla equipo solicitado existe
                        if (pr == null)
                        {
                            //Si no existe lo crea
                            pr = new EQUIPO_SOLICITADO();
                            pr.ID_PRESTAMO = id;
                            pr.TIPO_ACTIVO = y.NOMBRE;
                            pr.CANTIDAD = cantidad[a];
                            //Lo agrega a la tabla
                            if (ModelState.IsValid)
                            {
                                db.EQUIPO_SOLICITADO.Add(pr);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //Si si existe, lo modifica y guarda los cambios
                            EQUIPO_SOLICITADO eq = new EQUIPO_SOLICITADO();
                            decimal temp = cantidad[a];
                            noEsta = false;
                            eq.ID_PRESTAMO = pr.ID_PRESTAMO;
                            eq.TIPO_ACTIVO = y.NOMBRE;
                            eq.CANTIDAD = temp;
                            eq.CANTIDADAPROBADA = pr.CANTIDADAPROBADA;
                            db.EQUIPO_SOLICITADO.Remove(pr);
                            db.SaveChanges();
                            if (ModelState.IsValid)
                            {
                                db.EQUIPO_SOLICITADO.Add(eq);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                if (noEsta)
                {
                    //Si no se ha guardado en la tabla anteriormente lo crea y lo guarda
                    EQUIPO_SOLICITADO pr = new EQUIPO_SOLICITADO();
                    pr.ID_PRESTAMO = id;
                    pr.TIPO_ACTIVO = y.NOMBRE;
                    pr.CANTIDAD = cantidad[a];
                    if (ModelState.IsValid)
                    {
                        db.EQUIPO_SOLICITADO.Add(pr);
                        db.SaveChanges();
                    }
                }
                a++;
            }
            ViewBag.Mensaje = "El préstamo ha sido aprobado con éxito";

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
            ViewBag.fechSol = P.FECHA_SOLICITUD.ToShortDateString();

            //Guarda los cambios de los otros atributos de la tabla prestamo
            P.MOTIVO = p.MOTIVO;
            P.OBSERVACIONES_SOLICITANTE = p.OBSERVACIONES_SOLICITANTE;
            P.PERIODO_USO = p.PERIODO_USO;
            P.SIGLA_CURSO = p.SIGLA_CURSO;
            P.FECHA_RETIRO = p.FECHA_RETIRO;
            P.SOFTWARE_REQUERIDO = p.SOFTWARE_REQUERIDO;
            P.Estado = 1;
            if (ModelState.IsValid)
            {
                db.Entry(P).State = EntityState.Modified;
                db.SaveChanges();
                //Envia el correo de notificacion
                string subj = "Edición de Solicitud: "+numBol;
                var consultaUrl = Url.Action("Detalles", "PRESTAMOes", new { id = P.ID }, protocol: Request.Url.Scheme);
                string link = " " + consultaUrl + " ";
                string email = User.Identity.Name;
                string mensajito = "Se ha editado la solicitud con numero de boleta " + numBol + " exitosamente. Puede consultar esta solicitud en el siguiente link: "+link+" \n Gracias por preferirnos.\n";
                SolicitudBien(email, mensajito, subj);
                //Redirecciona al historial
                return RedirectToAction("Historial");
            }
            return View(P);
        }



        // GET: PRESTAMOes/Delete/5
        //Requiere: id del Préstamo
        //Modifica: Se encarga de cambiar el estado de la solicitud en la base de datos para que en prestamo aparezca cancelado.
        //Retorna: Vista con el resultado de dicha modificación en la base de datos.
        public ActionResult Delete(string id)
        {
            //Si no entra al cancelar de una solicitud en especifico da error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            //Si entra a cancelar una solicitud que no existe, da error
            if (pRESTAMO == null)
            {
                return HttpNotFound();
            }
            //Para determinar el estado en que se encuentra la solicitud en este momento
            ViewBag.Estadillo = "";
            ViewBag.Estadillo = "Cancelada";
            //Para determinar cual usuario fue el que hizo la solicitud
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
            //Para determinar las categorias de activos que existen
            var cat = (from ac in db.ACTIVOS
                       from t in db.TIPOS_ACTIVOS
                       where ac.PRESTABLE.Equals(true) &&
                              t.ID.Equals(ac.TIPO_ACTIVOID)
                       select new { t.NOMBRE, t.ID }).Distinct();
            //Para desplegar las cantidades solicitadas de cada categoria por ese usuario en esa solicitud especifica
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where (o.ID == id && o2.ID_PRESTAMO == id)
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            //Para crear la lista de equipo con cantidades que se va a desplegar en la tabla de equipo solicitado que se 
            //podra observar en la pantalla de cancelar (llamada Delete)
            var equipo = new List<List<String>>();
            foreach (var y in cat)
            {
                bool existeCategoria = false;
                List<String> temp = new List<String>();
                foreach (var x in equipo_sol)
                {

                    if (x.TIPO != null)
                    {
                        if (x.TIPO == y.NOMBRE)
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
            //la variable en la que se guarda la informacion a desplegarse en la tabla de equipo solicitado de la pantalla de cancelacion
            ViewBag.Equipo_Solict = equipo;
            return View(pRESTAMO);
        }

        // POST: PRESTAMOes/Delete/5
        //Requiere: id del Préstamo
        //Modifica: Se encarga de cambiar el estado de la solicitud en la base de datos para que en prestamo aparezca cancelado.
        //Retorna: Vista con el resultado de dicha modificación en la base de datos.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //busca el objeto PRESTAMO que tenga el id correspondiente
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);
            //Pone el estado del prestamos seleccionado en 6 (cancelado)
            pRESTAMO.Estado = 6;
            //Verifica que el modelo siga siendo valido despues del cambio
            if (ModelState.IsValid)
            {
                //Pone el estado del objeto prestamo seleccionado en modificado
                db.Entry(pRESTAMO).State = EntityState.Modified;
                //guarda los cambios en la base
                db.SaveChanges();
                //Enviar email notificando que se cancelo la solicitud
                string subj = "Cancelación de Solicitud: " + pRESTAMO.NUMERO_BOLETA;
                var consultaUrl = Url.Action("Detalles", "PRESTAMOes", new { id = pRESTAMO.ID }, protocol: Request.Url.Scheme);
                string link = " " + consultaUrl + " ";
                string email = User.Identity.Name;
                string mensajito = "Se ha cancelado la solicitud con numero de boleta " + pRESTAMO.NUMERO_BOLETA + " exitosamente. Puede consultar esta solicitud en el siguiente link: " + link + " \n Gracias por preferirnos.\n";
                SolicitudBien(email, mensajito, subj);
                //Redirecciona la pagina al historial
                return RedirectToAction("Historial");
            }
            //Redirecciona la pagina al historial
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
            Aprobada,
            Denegada,
            Abierta,
            Cerrada,
            Cancelada
        }

        public ActionResult Devolucion(string id)
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
            var equipo_sol = from o in db.PRESTAMOS
                             from o2 in db.EQUIPO_SOLICITADO
                             where o.ID == id && o2.CANTIDAD > 0
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };


            var equipo_cat = new Dictionary<String, List<List<String>>>();
            var categorias_sol = from p in db.PRESTAMOS
                                 from e in db.EQUIPO_SOLICITADO
                                 where p.ID == id && e.ID_PRESTAMO == p.ID
                                 select new { CAT = e.TIPO_ACTIVO, TIPO = e.TIPOS_ACTIVOSID };

            foreach (var c in categorias_sol)
            {
                var eq = new List<List<String>>();
                eq = equipoPorCategoria(c.TIPO, id);
                equipo_cat.Add(c.CAT.ToString(), eq);
            }


            var equipo = new List<List<String>>();
            foreach (var x in equipo_sol)
            {
                if (x.ID == id)
                {
                    if (x.ID == x.ID_EQUIPO)
                    {
                        List<String> temp = new List<String>();
                        if (x.TIPO != null) { temp.Add(x.TIPO.ToString()); } else { temp.Add(""); }
                        if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add(""); }
                        if (x.CANTAP != 0) { temp.Add(x.CANTAP.ToString()); } else { temp.Add(""); }
                        equipo.Add(temp);
                    }
                }
            }

            ViewBag.Equipo_Solict = equipo;
            ViewBag.EquipoPorCat = equipo_cat;

            return View(pRESTAMO);
        }




        [HttpPost]
        public ActionResult Devolucion(string ID, bool[] column5_checkbox, bool column5_checkAll, string b, string OBSERVACIONES_APROBADO)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(ID);
            pRESTAMO.OBSERVACIONES_APROBADO = OBSERVACIONES_APROBADO;
            if (ModelState.IsValid)
            {
                db.Entry(pRESTAMO).State = EntityState.Modified;
                db.SaveChanges();
            }

            var prestamo = db.PRESTAMOS.Include(i => i.EQUIPO_SOLICITADO).SingleOrDefault(h => h.ID == ID);
            var equipo_sol = prestamo.EQUIPO_SOLICITADO;
            var activos_asignados = prestamo.ACTIVOes;

            /*---------------------------------------------------------------------------*/
            if (b == "Actualizar devolución")
            {
                int cont = 0;
                if (!column5_checkAll)
                {
                    foreach (var y in equipo_sol)
                    {
                        bool t = column5_checkbox[cont];
                        if (t)
                        { //si fueron todos seleccionados en esa fila, de ese tipo
                            foreach (var x in activos_asignados)
                            {
                                if (x.TIPO_ACTIVOID == y.TIPOS_ACTIVOSID)
                                {
                                    x.ESTADO_PRESTADO = 0;
                                }
                            }
                        }
                        cont++;
                    }
                }
                else
                {
                    foreach (var y in equipo_sol)
                    {
                        foreach (var x in activos_asignados)
                        {
                            if (x.TIPO_ACTIVOID == y.TIPOS_ACTIVOSID)
                            {
                                x.ESTADO_PRESTADO = 0;
                            }
                        }
                    }
                }

                if (column5_checkAll) { pRESTAMO.Estado = 5; }

                if (ModelState.IsValid)
                {
                    db.Entry(pRESTAMO).State = EntityState.Modified;
                    db.SaveChanges();
                }
                bool hasErrors = ViewData.ModelState.Values.Any(x => x.Errors.Count > 1);
                if (!hasErrors)
                {
                    ViewBag.Mensaje = "Los activos han sido devueltos correctamente.";
                }
                else
                {
                    ViewBag.Mensaje2 = "Los activos no han sido devueltos correctamente.";
                }


            }



            var lista = db.PRESTAMOS.Include(i => i.EQUIPO_SOLICITADO).SingleOrDefault(h => h.ID == ID);
            ViewBag.Nombre = lista.USUARIO.NOMBRE;
            /*  -------------------------------------------------------------------------------------------  */

            var equipo = new List<List<String>>();
            foreach (var x in equipo_sol)
            {
                if (prestamo.ID == ID)
                {
                    if (prestamo.ID == x.ID_PRESTAMO)
                    {
                        List<String> temp = new List<String>();
                        if (x.TIPO_ACTIVO != null) { temp.Add(x.TIPO_ACTIVO.ToString()); } else { temp.Add(""); }
                        if (x.CANTIDAD != 0) { temp.Add(x.CANTIDAD.ToString()); } else { temp.Add(""); }
                        if (x.CANTIDADAPROBADA != 0) { temp.Add(x.CANTIDADAPROBADA.ToString()); } else { temp.Add(""); }
                        equipo.Add(temp);
                    }
                }
            }
            ViewBag.Equipo_Solict = equipo;

            /*  -------------------------------------------------------------------------------------------  */

            return View(pRESTAMO);
        }


        // Requiere: valor seleccionado en el dropdown de Categoría, valor del botón seleccionado, valor de la fecha inicial y la fecha final
        // Modifica: se encarga de llenar la tabla de Inventario, de la categoría que recibe cómo parámetro.
        // Regresa: N/A.
        private List<List<String>> llenarTablaDetails(String Categoria)
        {

            int tipo = int.Parse(Categoria);
            var activos_enCat = new List<List<String>>();
            var activos = db.ACTIVOS.Where(c => c.TIPO_ACTIVOID == tipo);
            foreach (Activos_PrestamosOET.Models.ACTIVO x in activos)
            {

                    if (Categoria.Equals(x.TIPO_ACTIVOID.ToString()) && x.PRESTABLE == true && x.ESTADO_PRESTADO== 0)
                    {
                    List<String> temp = new List<String>();
                    if (x.FABRICANTE != null) { temp.Add(x.FABRICANTE); } else { temp.Add(""); }
                    if (x.MODELO != null) { temp.Add(x.MODELO); } else { temp.Add(""); }
                    if (x.PLACA != null) { temp.Add(x.PLACA); } else { temp.Add(""); }

                    activos_enCat.Add(temp);
                }
            }
            
            if (activos_enCat.Count == 0)
            {
                List<String> temp = new List<String>();
                temp.Add("");
                temp.Add("");
                temp.Add("");
                activos_enCat.Add(temp);
                ViewBag.NoActivos = "No hay Activos Prestables con esta categoría.";
            }
            return activos_enCat;
    }
        private List<List<String>> llenarTablaDetails(String Categoria, string id)
        {


            var activos_enCat = new List<List<String>>();
            var activos = db.PRESTAMOS.Include(i => i.ACTIVOes).SingleOrDefault(h => h.ID == id);
            foreach (ACTIVO x in activos.ACTIVOes)
            {

                if (Categoria.Equals(x.TIPO_ACTIVOID.ToString()))
                    {
                    List<String> temp = new List<String>();
                    if (x.FABRICANTE != null) { temp.Add(x.FABRICANTE); } else { temp.Add(""); }
                    if (x.MODELO != null) { temp.Add(x.MODELO); } else { temp.Add(""); }
                    if (x.PLACA != null) { temp.Add(x.PLACA); } else { temp.Add(""); }

                    activos_enCat.Add(temp);
                }              
            }
            
            return activos_enCat;
        }

        protected void addActivosToPrestamo(string[] placas, string id)
        {
            LinkedList<ACTIVO> activosPorAgregar = new LinkedList<ACTIVO>();
            var prestamo = db.PRESTAMOS.Include(i => i.ACTIVOes).SingleOrDefault(h => h.ID == id);
            foreach (string p in placas)
            {
                if (p != "false")
                {
                    var activo = db.ACTIVOS.SingleOrDefault(i => i.PLACA == p);
                    activo.ESTADO_PRESTADO = 1;

                    prestamo.Estado = 4;
                    activosPorAgregar.AddLast(activo);
                    prestamo.ACTIVOes.Add(activo);
                    if (ModelState.IsValid)
                    {
                        db.Entry(activo).State = EntityState.Modified;
                        db.Entry(prestamo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors);
                    }

                }
            }
        }

        public byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }

        public void DownloadPDF(string viewName, object model, string nombreArchivo)
        {
            string HTMLContent = RenderRazorViewToString(viewName, model);

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(GetPDF(HTMLContent));
            Response.End();
        }

        /*   public static string RenderViewToString(string controllerName, string viewName, object viewData)
           {
               using (var writer = new StringWriter())
               {
                   var routeData = new RouteData();
                   routeData.Values.Add("controller", controllerName);
                   var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                   var razorViewEngine = new RazorViewEngine();
                   var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                   var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                   razorViewResult.View.Render(viewContext, writer);
                   return writer.ToString();
               } */

        public string RenderRazorViewToString(string viewName, object model)
        {
            // PRESTAMO pRESTAMO = db.PRESTAMOS.Find();
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult BoletaPDF(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);

            return View(pRESTAMO);
        }

    }
}
