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
            var prestamos = db.PRESTAMOS.Include(i => i.USUARIO);//Se agrega la tabla de usuarios a la de préstamos

            //Inician filtros de búsqueda
            if (!String.IsNullOrEmpty(fechaSolicitud) || !String.IsNullOrEmpty(fechaRetiro))//Caso en que se consulta por una fecha específica
            {
                DateTime fechaS;
                DateTime fechaR;


                if (String.IsNullOrEmpty(fechaSolicitud))//Se ingresó únicamente fecha de inicio del préstamo
                {
                    if (DateTime.TryParseExact(fechaRetiro, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaR))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_RETIRO.Value.Year == fechaR.Year
                                                          && model.FECHA_RETIRO.Value.Month == fechaR.Month
                                                          && model.FECHA_RETIRO.Value.Day == fechaR.Day);
                    }
                }
                else if (String.IsNullOrEmpty(fechaRetiro))//Se ingresó únicamente la fecha de solicitud del préstamo
                {
                    if (DateTime.TryParseExact(fechaSolicitud, "dd/MM/yyyy", new CultureInfo("es"), DateTimeStyles.None, out fechaS))
                    {
                        prestamos = prestamos.Where(model => model.FECHA_SOLICITUD.Value.Year == fechaS.Year
                                                          && model.FECHA_SOLICITUD.Value.Month == fechaS.Month
                                                          && model.FECHA_SOLICITUD.Value.Day == fechaS.Day);
                    }
                }
                else//Se ingresaron tanto la fecha de solicitud como de inicio del préstamo.
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
        //Requiere: Recibe el id del prestamo que se está consultando.
        // Modifica: Maneja el details view, la cual es la vista de consulta de revisión de una solicitud en particular.
        //Retorna: Devuelve un información necesaria para el despliegue de la vista como: nombre de solicitante, el estado, el equipo solicitado y sus cantidades
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

            //Segmento de código para colocar colores a las cantidad de solicitudes por categoría.
            var prestamos = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == id);//Se hace joint entre prestamos y equipo solicitado por id del préstamo           
            var equip = prestamos.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);//Se verifica que se seleccionen los equipos seleccionados que tengan más 0 soliciudes
            var fechas = db.PRESTAMOS.Include(j => j.EQUIPO_SOLICITADO).Where(p => p.FECHA_RETIRO <= prestamos.FECHA_RETIRO && p.ID != id);//Se selccionan las solicitudes de préstamo qe se encuentran "abiertos" para el momento de inicio del préstamo consultado.
            Dictionary<string, int> eq = new Dictionary<string, int>();//diccionario que almacena las cantidades de préstamos vigentes.
            foreach (var f in fechas)
            {
                if (f.FECHA_RETIRO.Value.AddDays(f.PERIODO_USO) >= prestamos.FECHA_RETIRO.Value)
                {
                    var equip2 = f.EQUIPO_SOLICITADO.Where(q => q.CANTIDAD > 0);//Se seleccionan pedidos co una cantidad mayor a 0
                    foreach (var e in equip2)
                    {
                        foreach (var pp in equip)
                        {
                            if (e.TIPO_ACTIVO == pp.TIPO_ACTIVO)//Si los prestamos "abiertos" tienen un articulo solicitado en el prestamo consultado, se registra la categoría y la cantidad
                            {//Se guarda en el diccionario
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
                //Se consulta la cantidad total de activos prestables de una categoría 
                int contador = (from a in db.ACTIVOS
                                where a.TIPO_ACTIVOID == tipo
                                select a).Count();
                int total = 0;
                if (eq.ContainsKey(e.TIPO_ACTIVO))
                {

                    total = Convert.ToInt32(e.CANTIDAD) + eq[e.TIPO_ACTIVO];//
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


        //Requiere: Recibe el id del prestamo que se está consultando, un vector desde la vista que envia las cantidades de equipo solicitado y un string q se usa para determinar que boton fue apretado.
        // Modifica: Maneja el details view, la cual es la vista de consulta de revisión de una solicitud en particular.
        //Retorna: Devuelve un información necesaria para el despliegue de la vista como: nombre de solicitante, el estado, el equipo solicitado y sus cantidades, además, despliega un mensaje de confirmacion diferente de acuerdo a si el boton fue aceptar o denegar

        [HttpPost]
        public ActionResult Details(string ID, int[] cantidad_aprobada, string b)
        {
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(ID);

            var prestamo = db.PRESTAMOS.Include(i => i.EQUIPO_SOLICITADO).SingleOrDefault(p => p.ID == ID);

            var equipo_sol = prestamo.EQUIPO_SOLICITADO;

            if (b == "Aceptar")
            {

                int a = 0;
                foreach (var x in equipo_sol)
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


                        if (x.CANTIDAD != 0)
                        {
                            temp.Add(x.CANTIDAD.ToString());
                        }
                        else
                        {
                            temp.Add("");
                        }

                        if (x.CANTAP != 0)
                        {
                            temp.Add(x.CANTAP.ToString());
                        }
                        else
                        {
                            temp.Add("");
                        }
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
                prestamo.NUMERO_BOLETA = 1;// calcularNumBoleta();
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
            ViewBag.fechSol = viewBagFechaSolicitada(pRESTAMO.FECHA_SOLICITUD.Value.Date);
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
                             where (o.ID == id && o2.ID_PRESTAMO == id )
                             select new { ID = o.ID, ID_EQUIPO = o2.ID_PRESTAMO, TIPO = o2.TIPO_ACTIVO, CANTIDAD = o2.CANTIDAD, CANTAP = o2.CANTIDADAPROBADA };
            //Acomoda la informacion de la tabla de equipo solicitado que se desplegara en la pantalla
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
                    if (y.ID.ToString() == x.TIPO)
                    {
                        EQUIPO_SOLICITADO pr = db.EQUIPO_SOLICITADO.Find(id, y.ID.ToString(), x.CANTIDAD);
                        //busca si el elemento de la tabla equipo solicitado existe
                        if (pr == null)
                        {
                            //Si no existe lo crea
                            pr = new EQUIPO_SOLICITADO();
                            pr.ID_PRESTAMO = id;
                            pr.TIPO_ACTIVO = y.ID.ToString();
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
                            eq.TIPO_ACTIVO = pr.TIPO_ACTIVO;
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
                    pr.TIPO_ACTIVO = y.ID.ToString();
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
            ViewBag.fechSol = P.FECHA_SOLICITUD.Value.ToShortDateString();

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
    }
}
