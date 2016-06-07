using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Local.Controllers
{
    public class InventarioController : Controller
    {
        private Activos_PrestamosOET.Models.PrestamosEntities db = new Activos_PrestamosOET.Models.PrestamosEntities();

        // GET: Inventario/Index
        // default
        // Requiere: N/A.
        // Modifica: muestra la información al ingresar a la página de Inventario inicialmente.
        // Regresa: vista con la tabla de los datos acerca del Inventario.
        public ActionResult Index()
        {
            llenarTablaInventario();
            return View();
        }

        
        // Requiere: valor seleccionado en el dropdown de Categoría, valor del botón seleccionado, valor de la fecha inicial y la fecha final
        // Modifica: muestra después de seleccionado algún botón, los resultados correspondientes, mostrando las tablas que corresponden.
        // Regresa: vista con las tablas cargadas que corresponden.
        [HttpPost]
        public ActionResult Index(String dropdownCategoria, String submit, String datepicker, String datepicker1)
        {
            if (!string.IsNullOrEmpty(submit) && submit.Equals("Buscar"))
            {
                if (!dropdownCategoria.Equals("1"))
                {
                    llenarTabla(dropdownCategoria, datepicker, datepicker1);
                    return View();
                }//---------------------------------------------------------------------------------------------------------------------
                else
                {
                    llenarTablaInventario();
                    return View();
                }
            }//--------------------------------------------------------------------------------------------------------
            //---------------------------------------------------------------------------------------------------------
            else
            {
                llenarTablaCategoria(datepicker, datepicker1, dropdownCategoria);
                return View();
            }

        }

        // Requiere: N/A.
        // Modifica: se encarga de llenar la tabla de Inventario, de todas las categorías para ACTIVOS.
        // Regresa: N/A.
        private void llenarTablaInventario() {
            List<String> solicitantes = new List<String>();
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


            var courses = new List<List<String>>();
            var users = db.ACTIVOS;
            foreach (Activos_PrestamosOET.Models.ACTIVO x in users)
            {
                if (x.PRESTABLE == true)
                {
                    List<String> temp = new List<String>();

                    if (x.FABRICANTE != null) { temp.Add(x.FABRICANTE); } else { temp.Add(""); }
                    if (x.MODELO != null) { temp.Add(x.MODELO); } else { temp.Add(""); }
                    if (x.PLACA != null) { temp.Add(x.PLACA); } else { temp.Add(""); }

                    if (x.TIPO_ACTIVOID != 0)
                    {
                        foreach (Activos_PrestamosOET.Models.TIPOS_ACTIVOS tipos in db.TIPOS_ACTIVOS)
                        {
                            if (x.TIPO_ACTIVOID.Equals(tipos.ID))
                            {
                                temp.Add(tipos.NOMBRE);

                            }
                        }
                    }
                    else { temp.Add(""); }

                    if (x.DESCRIPCION != null) { temp.Add(x.DESCRIPCION); } else { temp.Add(""); }

                    String prestado_a = "No prestado";
                    String prestado_hasta = "Sin fecha";
                    foreach (Activos_PrestamosOET.Models.PRESTAMO f in x.PRESTAMOes)
                    {
                        if (ceds.Contains(f.CED_SOLICITA) && f.CED_SOLICITA != null)
                        {
                            prestado_a = solicitantes[ceds.IndexOf(f.CED_SOLICITA)];
                        }
                        prestado_hasta = f.FECHA_RETIRO.ToString();
                    }
                    temp.Add(prestado_a);
                    temp.Add(prestado_hasta);


                    courses.Add(temp);
                }
            }
            ViewBag.Courses = courses;
            if (courses.Count == 0)
            {
                ViewBag.Mensaje0 = "No hay Activos Prestables.";
            }
        }

        // Requiere: valor seleccionado en el dropdown de Categoría, valor del botón seleccionado, valor de la fecha inicial y la fecha final
        // Modifica: Se encarga de llenar las tablas de Inventario y la de categoría, basadas en las selecciones del usuario.
        // Regresa: N/A.
        private void llenarTablaCategoria(String datepicker, String datepicker1, String dropdownCategoria) {
            var courses = new List<List<String>>();
            var viewModel = from o in db.EQUIPO_SOLICITADO.ToList()
                            join o2 in db.PRESTAMOS.ToList()
                            on o.ID_PRESTAMO equals o2.ID
                            where o.ID_PRESTAMO.Equals(o2.ID)
                            select new Inventario.Models.ModeloInventario { Equipos = o, Prestamos = o2 };
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int i = 0;

            string[] keys;
            string[] values;
            foreach (Inventario.Models.ModeloInventario mi in viewModel)
            {


                DateTime dt = DateTime.ParseExact(datepicker, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt1 = DateTime.ParseExact(datepicker1, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (mi.Prestamos.FECHA_SOLICITUD > dt && mi.Prestamos.FECHA_SOLICITUD < dt1)
                {
                    foreach (Activos_PrestamosOET.Models.TIPOS_ACTIVOS tipos in db.TIPOS_ACTIVOS)
                    {
                        if (mi.Equipos.TIPO_ACTIVO.Equals(tipos.ID.ToString()))
                        {
                            if (dictionary.ContainsKey(tipos.NOMBRE))
                            {
                                int value = Convert.ToInt32(dictionary[tipos.NOMBRE]);
                                value += Convert.ToInt32(mi.Equipos.CANTIDAD);
                                dictionary[tipos.NOMBRE] = value.ToString();
                            }
                            else
                            {
                                dictionary.Add(tipos.NOMBRE, mi.Equipos.CANTIDAD.ToString());
                            }
                        }
                    }
                }
            }
            keys = dictionary.Keys.ToArray();
            values = dictionary.Values.ToArray();
            for (; i < keys.Length; i++)
            {
                List<String> temp = new List<String>();
                temp.Add(keys[i]);
                temp.Add(values[i]);
                courses.Add(temp);
            }
            if (keys.Length == 0)
            {
                ViewBag.Mensaje2 = "No hay préstamos en estas fechas.";
            }




            ViewBag.Courses1 = courses;
            llenarTablaInventario();
        }

        // Requiere: valor seleccionado en el dropdown de Categoría, valor del botón seleccionado, valor de la fecha inicial y la fecha final
        // Modifica: se encarga de llenar la tabla de Inventario, de la categoría que recibe cómo parámetro.
        // Regresa: N/A.
        private void llenarTabla(String dropdownCategoria, String datepicker, String datepicker1) {
            var viewModel = from o in db.PRESTAMOS.ToList()
                            join o2 in db.USUARIOS.ToList()
                                on o.CED_SOLICITA equals o2.CLAVE
                            where o.CED_SOLICITA.Equals(o2.CLAVE)
                            select new Inventario.Models.ModeloInventario { Prestamos = o, Usuarios = o2 };


            var courses = new List<List<String>>();
            var users = db.ACTIVOS;
            foreach (Activos_PrestamosOET.Models.ACTIVO x in users)
            {
                int cat = 0;
                foreach (Activos_PrestamosOET.Models.TIPOS_ACTIVOS tipos in db.TIPOS_ACTIVOS)
                {
                    if (dropdownCategoria.Equals(tipos.NOMBRE))
                    {
                        cat = tipos.ID;

                    }
                }

                if (x.PRESTABLE == true && x.TIPO_ACTIVOID.Equals(cat))
                {
                    List<String> temp = new List<String>();

                    if (x.FABRICANTE != null) { temp.Add(x.FABRICANTE); } else { temp.Add(""); }
                    if (x.MODELO != null) { temp.Add(x.MODELO); } else { temp.Add(""); }
                    if (x.PLACA != null) { temp.Add(x.PLACA); } else { temp.Add(""); }
                    if (x.TIPO_ACTIVOID != 0)
                    {
                        foreach (Activos_PrestamosOET.Models.TIPOS_ACTIVOS tipos in db.TIPOS_ACTIVOS)
                        {
                            if (x.TIPO_ACTIVOID.Equals(tipos.ID))
                            {
                                temp.Add(tipos.NOMBRE);

                            }
                        }
                    }
                    else { temp.Add(""); }

                    if (x.DESCRIPCION != null) { temp.Add(x.DESCRIPCION); } else { temp.Add(""); }

                    String prestado_a = "No prestado";
                    String prestado_hasta = "Sin fecha";
                    foreach (Activos_PrestamosOET.Models.PRESTAMO f in x.PRESTAMOes)
                    {
                        foreach (Inventario.Models.ModeloInventario mi in viewModel)
                        {
                            if (f.CED_SOLICITA.Equals(mi.Usuarios.CLAVE)
                                && f.CED_SOLICITA != null)
                            {
                                prestado_a = mi.Usuarios.NOMBRE;
                            }
                            prestado_hasta = f.FECHA_RETIRO.ToString();
                        }

                    }
                    temp.Add(prestado_a);
                    temp.Add(prestado_hasta);

                    //if viewModel tiene  hace match con alguno de los f's
                    courses.Add(temp);
                }
            }
            ViewBag.Courses = courses;
            if (courses.Count==0)
            {
                ViewBag.Mensaje1 = "No hay Activos Prestables con esta categoría.";
            }
        }

        public ActionResult Details(string id)
        {
            var pRESTAMO = db.ACTIVOS.Include(p => p.PRESTAMOes).SingleOrDefault(m => m.PLACA == id);
            return View(pRESTAMO);
        }
    }
}