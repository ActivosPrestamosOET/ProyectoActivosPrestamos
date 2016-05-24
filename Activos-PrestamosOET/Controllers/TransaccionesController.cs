using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Activos_PrestamosOET.Models;

namespace Activos_PrestamosOET.Controllers
{
    public class TransaccionesController : Controller
    {

        private PrestamosEntities db = new PrestamosEntities();

        public bool Create(string responsable, string estado, string descripcion, string activo_id)
        {
            TRANSACCION nueva_transaccion = new TRANSACCION();
            nueva_transaccion.FECHA = DateTime.Now;
            nueva_transaccion.ESTADO = estado;
            nueva_transaccion.DESCRIPCION = descripcion;
            nueva_transaccion.ACTIVOID = activo_id;
            nueva_transaccion.RESPONSABLE = responsable;


            if (ModelState.IsValid)
            {
                db.TRANSACCIONES.Add(nueva_transaccion);
                db.SaveChanges();
                return true;
            }

            return false;
        }

    }
}