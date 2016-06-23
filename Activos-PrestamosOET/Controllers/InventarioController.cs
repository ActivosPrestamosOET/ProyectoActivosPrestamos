using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Activos_PrestamosOET.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


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
        public ActionResult Index(String dropdownCategoria, String submit, String datepicker, String datepicker1, string b)
        {
            //Si se presiona el boton de descargar la boleta
            if (b == "Reporte Activos")
            {
                //var temp = db.ACTIVOS.Where(x => x.PRESTABLE == true).ToList();
                //DownloadPDF("BoletaPDF", temp, "BoletaSoliciud");
                ExportToExcel();
            }
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
            else if (!string.IsNullOrEmpty(datepicker))
            {
                llenarTablaCategoria(datepicker, datepicker1, dropdownCategoria);
                return View();
            }
            else {
                return RedirectToAction("Inventario");
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
                foreach (Activos_PrestamosOET.Models.ActivosUser u in db.ActivosUsers)
                {
                    if (p.USUARIO_SOLICITA != null)
                    {
                        if (p.USUARIO_SOLICITA.Equals(u.Cedula))
                        {
                            solicitantes.Add(u.Nombre);
                            ceds.Add(u.Cedula);
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
                        if (ceds.Contains(f.USUARIO_SOLICITA) && f.USUARIO_SOLICITA != null)
                        {
                            prestado_a = solicitantes[ceds.IndexOf(f.USUARIO_SOLICITA)];
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
                            join o2 in db.ActivosUsers.ToList()
                                on o.USUARIO_SOLICITA equals o2.Cedula
                            where o.USUARIO_SOLICITA.Equals(o2.Cedula)
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
                            if (f.USUARIO_SOLICITA.Equals(mi.Usuarios.Cedula)
                                && f.USUARIO_SOLICITA != null)
                            {
                                prestado_a = mi.Usuarios.Nombre;
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

        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activo = db.ACTIVOS.Include(p => p.PRESTAMOes).Include(p => p.TRANSACCIONES).SingleOrDefault(m => m.PLACA == id);

            return View(activo);
        }

        public ActionResult DescargarHistorial(string id)
        {
            var activo = db.ACTIVOS.Include(p => p.PRESTAMOes).Include(p => p.TRANSACCIONES).SingleOrDefault(m => m.PLACA == id);
            DownloadPDF("DetailsPDF", activo, "HistorialActivo");
                 return RedirectToAction("Details", new { id = id });

        }
        public ActionResult DetailsPDF(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activo = db.ACTIVOS.Include(p => p.PRESTAMOes).Include(p => p.TRANSACCIONES).SingleOrDefault(m => m.PLACA == id);

            //Si se presiona el boton de descargar la boleta

            return View(activo);

        }


        //------------------------------------------------------------------------------------------------------

        //Requiere: la vista, el modelo al que pertenece la vista, el nombre que se quiere que tenga el archivo
        //Modifica: se encarga de organizar la entrada de una vista, llamar al metodo que lo convierte a un doc itextsharp y que se pueda descargar como PDF
        //Regresa: N/A

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

        //Requiere: la vista, el modelo al que pertenece la vista
        //Modifica: convierte la vista en un string para que pueda ser leido por itextsharp
        //Regresa: un string con la informacion de la vista
        public string RenderRazorViewToString(string viewName, object model)
        {
            // PRESTAMO pRESTAMO = db.PRESTAMOS.Find();
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        //Requiere: una string parseada de la vista que se quiere convertir en PDF
        //Modifica: se encarga de pasar la vista HTML a un documento de itextsharp
        //Regresa: un byte con el documento 
        public byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // Se crea un documneto de itextsharp
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // el htmlworker parsea el documento
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            doc.Open();
            htmlWorker.StartDocument();

            // parsea el html en el doc
            htmlWorker.Parse(txtReader);

            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }

        //Requiere: el id del prestamo 
        //Modifica: se encarga de llamar a la vista que luego se convertira en la boleta imprimible de un prestamo
        //Regresa: la vista
        public ActionResult BoletaPDF(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRESTAMO pRESTAMO = db.PRESTAMOS.Find(id);

            return View(pRESTAMO);
        }

        public ActionResult ExportarExcel(string id)
        {
            GridView gv = new GridView();
            var activo = db.ACTIVOS.Include(p => p.PRESTAMOes).Include(p => p.TRANSACCIONES).SingleOrDefault(m => m.PLACA == id);
            gv.DataSource = activo.PRESTAMOes;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=HistorialActivo.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(activo);

        }

        /*
                 public ActionResult ExportToExcel()
                {
                    var temp = db.ACTIVOS.Include(p => p.PRESTAMOes).Include(p => p.TRANSACCIONES).SingleOrDefault(m => m.PLACA == id);


                    var grid = new GridView();
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("Número de Boleta", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Fecha de Retiro", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Fecha de Devolución", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Tipo", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Descripcion", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Prestado_a", Type.GetType("System.String")));
                    dt.Columns.Add(new DataColumn("Prestado_hasta", Type.GetType("System.String")));

                    foreach (var item in temp)
                    {
                        DataRow dr = dt.NewRow();
                        if (item.NUMERO_BOLETA == null)
                        {
                            dr["Número de Boleta"] = "No tiene Número de Boleta especificado";
                        }
                        else
                        {
                            dr["Número de Boleta"] = item.NUMERO_BOLETA;
                        }
                        if (item.FECHA_RETIRO == null)
                        {
                            dr["Fecha de Retiro"] = "No tiene Fecha de Retiro especificado";
                        }
                        else
                        {
                            dr["Fecha de Retiro"] = item.FECHA_RETIRO;
                        }
                        if (item.FECHA_RETIRO == null)
                        {
                            dr["Fecha de Devolución"] = "No tiene Fecha de Devolución especificada";
                        }
                        else
                        {
                            dr["Fecha de Devolución"] = item.FECHA_RETIRO.AddDays(item.PERIODO_USO).ToShortDateString();
                        }
                        if (item.ActivosUser.Nombre == null)
                        {
                            dr["Solicitante"] = "No tiene Solicitante especificado";
                        }
                        else
                        {
                            dr["Solicitante"] = item.ActivosUser.Nombre;
                        }
                        foreach (var x in temp.TRANSACCIONES)
                        {

                        if (x.ACTIVOID == temp.ID && x.NUMERO_BOLETA == item.NUMERO_BOLETA)
                        {
                            dr["Observaciones al devolver"] = x.OBSERVACIONES_RECIBO;

                        }					
                        else (x == null)
                        {
                                dr["Observaciones al devolver"] = "No ha sido prestado";
                        }

                        }
                        dt.Rows.Add(dr);
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    grid.DataSource = ds.Tables[0];
                    grid.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=HistorialActivo.xls");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();

                    return View(temp);
                }
        */

        public ActionResult ExportToExcel()
        {
            var temp = db.ACTIVOS.Where(x => x.PRESTABLE == true).ToList();

            var grid = new GridView();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Fabricante", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Modelo", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Placa", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Tipo", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Descripcion", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Prestado_a", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Prestado_hasta", Type.GetType("System.String")));

            foreach (var item in temp)
            {
                DataRow dr = dt.NewRow();
                if (item.FABRICANTE == null)
                {
                    dr["Fabricante"] = "No tiene fabricante especificado";
                }
                else
                {
                    dr["Fabricante"] = item.FABRICANTE;
                }
                if (item.MODELO == null)
                {
                    dr["Modelo"] = "No tiene modelo especificado";
                }
                else
                {
                    dr["Modelo"] = item.MODELO;
                }
                if (item.PLACA == null)
                {
                    dr["Placa"] = "No tiene placa especificada";
                }
                else
                {
                    dr["Placa"] = item.PLACA;
                }
                if (item.TIPOS_ACTIVOS == null)
                {
                    dr["Tipo_activo"] = "No tiene Tipo de Activo especificado";
                }
                else
                {
                    dr["Tipo"] = item.TIPOS_ACTIVOS.NOMBRE;
                }
                if (item.DESCRIPCION == null)
                {
                    dr["Descripcion"] = "No tiene descripcion especificada";
                }
                else
                {
                    dr["Descripcion"] = item.DESCRIPCION;
                }
                foreach (var x in item.PRESTAMOes)
                {
                    if (x == null)
                    {
                        dr["Prestado_a"] = "No ha sido prestado";
                        dr["Prestado_hasta"] = "No ha sido prestado";
                    }
                    else
                    {
                        dr["Prestado_a"] = x.ActivosUser.Nombre;
                        dr["Prestado_hasta"] = x.FECHA_RETIRO;
                    }
                }
                dt.Rows.Add(dr);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            grid.DataSource = ds.Tables[0];
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ReporteActivos.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
    }
}