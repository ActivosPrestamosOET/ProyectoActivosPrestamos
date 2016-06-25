using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Activos_PrestamosOET.Models;

namespace Activos_PrestamosOET.Controllers
{
    public class EmpleadosController : Controller
    {
        private PrestamosEntities db = new PrestamosEntities();

        // GET: Empleados
        public ActionResult Index()
        {
            return View(db.V_EMPLEADOS.ToList());
        }

        // GET: Empleados/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            V_EMPLEADOS v_EMPLEADOS = db.V_EMPLEADOS.Find(id);
            if (v_EMPLEADOS == null)
            {
                return HttpNotFound();
            }
            return View(v_EMPLEADOS);
        }

        /**
         * Metodo que filtra los empleados con base en una estacion.
         * @param: El identificador de la estacion
         * @return: Los usuarios que pertenecen a la estacion seleccionada, que estan activos en la empresa y que poseen correo electronico.
         **/
        public static IQueryable<V_EMPLEADOS> EmpleadosFiltrados(string id_estacion)
        {
            PrestamosEntities bd = new PrestamosEntities();
            var empleados = bd.V_EMPLEADOS.Where(emp => emp.ESTACION_ID.Equals(id_estacion) && emp.ESTADO.Equals(1) && emp.EMAIL.Contains("@")).OrderBy(emp => emp.NOMBRE).ToList().Select(empleado => new V_EMPLEADOS
            {
                IDEMPLEADO = empleado.IDEMPLEADO,
                NOMBRE = empleado.NOMBRE
            }).AsQueryable();
            return empleados;
        }
    }
}
