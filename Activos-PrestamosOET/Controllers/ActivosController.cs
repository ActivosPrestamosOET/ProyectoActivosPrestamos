using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Activos_PrestamosOET.Models;
using PagedList;

namespace Activos_PrestamosOET.Controllers
{
    public class ActivosController : Controller
    {
        private PrestamosEntities db = new PrestamosEntities();

        // GET: Activos
        public ActionResult Index(string orden, string filtro, string busqueda, string V_PROVEEDORIDPROVEEDOR, string TIPO_ACTIVOID, string V_ANFITRIONAID, string TIPO_TRANSACCIONID, string ESTADO_ACTIVOID, string V_ESTACIONID, string fecha_antes, string fecha_despues, string usuario, string fabricante, int? pagina)
        {

            ViewBag.OrdenActual = orden;
            ViewBag.NumPlacaParam = String.IsNullOrEmpty(orden) ? "num_placa_desc" : "";
            ViewBag.EstadoParam = (orden == "Estado") ? "estado_desc" : "Estado" ;

            var aCTIVOS = from a in db.ACTIVOS select a;

            // Paginación
            if (busqueda != null)
            {
                pagina = 1;
            }
            else
            {
                busqueda = filtro;
            }

            ViewBag.FiltroActual = busqueda;

            // Busqueda con base en los parametros que ingresa el usuario
            #region Busqueda simple
            if (!String.IsNullOrEmpty(busqueda))
            {
                aCTIVOS = aCTIVOS.Where(a => a.ESTADOS_ACTIVOS.NOMBRE.Contains(busqueda)
                                            || a.NUMERO_SERIE.Contains(busqueda)
                                            || a.V_ANFITRIONA.NOMBRE.Contains(busqueda)
                                            || a.V_ESTACION.NOMBRE.Contains(busqueda)
                                            || a.TIPOS_ACTIVOS.NOMBRE.Contains(busqueda)
                                            || a.FABRICANTE.Contains(busqueda));
            }
            #endregion

            #region Busqueda avanzada

            // Para las opciones de busqueda avanzada
            ViewBag.TIPO_TRANSACCIONID = new SelectList(db.TIPOS_TRANSACCIONES, "ID", "NOMBRE");
            ViewBag.TIPO_ACTIVOID = new SelectList(db.TIPOS_ACTIVOS, "ID", "NOMBRE");
            ViewBag.V_PROVEEDORIDPROVEEDOR = new SelectList(db.V_PROVEEDOR, "IDPROVEEDOR", "NOMBRE");
            ViewBag.V_ANFITRIONAID = new SelectList(db.V_ANFITRIONA, "ID", "NOMBRE");
            ViewBag.ESTADO_ACTIVOID = new SelectList(db.ESTADOS_ACTIVOS, "ID", "NOMBRE");
            ViewBag.V_ESTACIONID = new SelectList(db.V_ESTACION, "ID", "NOMBRE");

            if (!String.IsNullOrEmpty(V_PROVEEDORIDPROVEEDOR))
            {
                aCTIVOS = aCTIVOS.Where(a => a.V_PROVEEDORIDPROVEEDOR.Equals(V_PROVEEDORIDPROVEEDOR));
            }
            if (!String.IsNullOrEmpty(TIPO_ACTIVOID))
            {
                Int32 id = Convert.ToInt32(TIPO_ACTIVOID);
                aCTIVOS = aCTIVOS.Where(a => a.TIPO_ACTIVOID.Equals(id));
            }
            if (!String.IsNullOrEmpty(V_ANFITRIONAID))
            {
                aCTIVOS = aCTIVOS.Where(a => a.V_ANFITRIONAID.Equals(V_ANFITRIONAID));
            }
            if (!String.IsNullOrEmpty(TIPO_TRANSACCIONID))
            {
                Int32 id = Convert.ToInt32(TIPO_TRANSACCIONID);
                aCTIVOS = aCTIVOS.Where(a => a.TIPO_TRANSACCIONID.Equals(id));
            }
            if (!String.IsNullOrEmpty(fecha_antes))
            {
                DateTime fecha = Convert.ToDateTime(fecha_antes);
                aCTIVOS = aCTIVOS.Where(a => a.FECHA_COMPRA.CompareTo(fecha) < 0);
            }
            if (!String.IsNullOrEmpty(fecha_despues))
            {
                DateTime fecha = Convert.ToDateTime(fecha_despues);
                aCTIVOS = aCTIVOS.Where(a => a.FECHA_COMPRA.CompareTo(fecha) > 0);
            }
            if (!String.IsNullOrEmpty(usuario))
            {
                aCTIVOS = aCTIVOS.Where(a => a.INGRESADO_POR.Contains(usuario));
            }
            if (!String.IsNullOrEmpty(ESTADO_ACTIVOID))
            {
                Int32 id = Convert.ToInt32(ESTADO_ACTIVOID);
                aCTIVOS = aCTIVOS.Where(a => a.ESTADO_ACTIVOID.Equals(id));
            }
            if (!String.IsNullOrEmpty(V_ESTACIONID))
            {
                aCTIVOS = aCTIVOS.Where(a => a.V_ESTACIONID.Equals(V_ESTACIONID));
            }
            if (!String.IsNullOrEmpty(fabricante))
            {
                aCTIVOS = aCTIVOS.Where(a => a.FABRICANTE.Contains(fabricante));
            }
            #endregion

            switch (orden)
            {
                case "num_placa_desc":
                    aCTIVOS = aCTIVOS.OrderByDescending(a => a.PLACA);
                    break;
                case "Estado":
                    aCTIVOS = aCTIVOS.OrderBy(a => a.ESTADOS_ACTIVOS.NOMBRE);
                    break;
                case "estado_desc":
                    aCTIVOS = aCTIVOS.OrderByDescending(a => a.ESTADOS_ACTIVOS.NOMBRE);
                    break;
                default:
                    aCTIVOS = aCTIVOS.OrderBy(a => a.PLACA);
                    break;
            }

            int tamano_pagina = 6;
            int num_pagina = (pagina ?? 1);

            return View(aCTIVOS.ToPagedList(num_pagina, tamano_pagina));
        }

        // GET: Activos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTIVO aCTIVO = db.ACTIVOS.Find(id);
            var tRANSACCION = from a in db.TRANSACCIONES select a;

            if (aCTIVO == null)
            {
                return HttpNotFound();
            }

            tRANSACCION = tRANSACCION.Where(a => a.ACTIVOID.Equals(id));

            aCTIVO.TRANSACCIONES = new HashSet<TRANSACCION>(tRANSACCION);

            return View(aCTIVO);
        }

        // GET: Activos/Create
        public ActionResult Create()
        {
            ViewBag.TIPO_TRANSACCIONID = new SelectList(db.TIPOS_TRANSACCIONES, "ID", "NOMBRE");
            ViewBag.TIPO_ACTIVOID = new SelectList(db.TIPOS_ACTIVOS, "ID", "NOMBRE");
            ViewBag.V_PROVEEDORIDPROVEEDOR = new SelectList(db.V_PROVEEDOR, "IDPROVEEDOR", "NOMBRE");
            ViewBag.V_ANFITRIONAID = new SelectList(db.V_ANFITRIONA, "ID", "NOMBRE");
            ViewBag.FECHA_INGRESO = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.V_MONEDAID = new SelectList(db.V_MONEDA, "ID", "SIMBOLO");

            return View();
        }

        // POST: Activos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NUMERO_SERIE,FECHA_COMPRA,INICIO_SERVICIO,FECHA_INGRESO,FABRICANTE,PRECIO,DESCRIPCION,EXENTO,PRESTABLE,TIPO_CAPITAL,INGRESADO_POR,NUMERO_DOCUMENTO,NUMERO_LOTE,TIPO_TRANSACCIONID,ESTADO_ACTIVOID,TIPO_ACTIVOID,COMENTARIO,DESECHADO,MODELO,V_USUARIOSIDUSUARIO,V_ESTACIONID,V_ANFITRIONAID,V_PROVEEDORIDPROVEEDOR,V_MONEDAID,CENTRO_DE_COSTOId,PLACA,ESTADO_PRESTADO")] ACTIVO aCTIVO)
        {

            var estado = db.ESTADOS_ACTIVOS.ToList().Where(ea => ea.NOMBRE == "Disponible");
            aCTIVO.ESTADO_ACTIVOID = estado.ToList()[0].ID;

            /* TODO: La manera correcta de hacer esto es en el modelo (en el constructor). Para al siguiente sprint se debe pasar esto 
             * al modelo para respetar el MVC.
             */
            decimal precio;
            if (Convert.ToBoolean(Request["MONEDA"]))
            {
                // Colones
                decimal tipo_cambio = db.V_TIPO_CAMBIO.ToList()[0].TIPOCAMBIO;
                precio = aCTIVO.PRECIO / tipo_cambio;

            }
            else
            {
                //Dolares
                precio = aCTIVO.PRECIO;

            }
            aCTIVO.TIPO_CAPITAL = (precio >= 1000) ? true : false;
            // Hasta acá debe de ir en el modelo.

            if (ModelState.IsValid)
            {
                db.ACTIVOS.Add(aCTIVO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TIPO_TRANSACCIONID = new SelectList(db.TIPOS_TRANSACCIONES, "ID", "NOMBRE", aCTIVO.TIPO_TRANSACCIONID);
            ViewBag.TIPO_ACTIVOID = new SelectList(db.TIPOS_ACTIVOS, "ID", "NOMBRE", aCTIVO.TIPO_ACTIVOID);
            ViewBag.V_PROVEEDORIDPROVEEDOR = new SelectList(db.V_PROVEEDOR, "IDPROVEEDOR", "NOMBRE", aCTIVO.V_PROVEEDORIDPROVEEDOR);
            ViewBag.V_ANFITRIONAID = new SelectList(db.V_ANFITRIONA, "ID", "NOMBRE", aCTIVO.V_ANFITRIONAID);
            ViewBag.V_MONEDAID = new SelectList(db.V_MONEDA, "ID", "SIMBOLO", aCTIVO.V_MONEDAID);

            return View(aCTIVO);
        }

        // GET: Activos/Asignar/7
        public ActionResult Asignar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // no deberia cargar datos de la asignacion pasada, cada asignacion es nueva
            // si quiero ver a quien esta asignado nada mas puedo ver los detalles del activo
            ACTIVO aCTIVO = db.ACTIVOS.Find(id);
            aCTIVO.COMENTARIO = "";
            if (aCTIVO == null)
            {
                return HttpNotFound();
            }
            ViewBag.V_USUARIOSIDUSUARIO = new SelectList(db.V_USUARIOS, "IDUSUARIO", "NOMBRE", aCTIVO.V_USUARIOSIDUSUARIO);
            ViewBag.ESTADO_ACTIVOID = new SelectList(db.ESTADOS_ACTIVOS, "ID", "NOMBRE", aCTIVO.ESTADO_ACTIVOID);
            ViewBag.V_ESTACIONID = new SelectList(db.V_ESTACION, "ID", "NOMBRE", aCTIVO.V_ESTACIONID);
            ViewBag.CENTRO_DE_COSTOId = new SelectList(db.CENTROS_DE_COSTOS, "ID", "NOMBRE", aCTIVO.CENTRO_DE_COSTOId);
            return View(aCTIVO);
        }

        // POST: Activos/Asignar/7
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar([Bind(Include = "ID,NUMERO_SERIE,FECHA_COMPRA,INICIO_SERVICIO,FECHA_INGRESO,FABRICANTE,PRECIO,DESCRIPCION,EXENTO,PRESTABLE,TIPO_CAPITAL,INGRESADO_POR,NUMERO_DOCUMENTO,NUMERO_LOTE,TIPO_TRANSACCIONID,ESTADO_ACTIVOID,TIPO_ACTIVOID,COMENTARIO,DESECHADO,MODELO,V_USUARIOSIDUSUARIO,V_ESTACIONID,V_ANFITRIONAID,V_PROVEEDORIDPROVEEDOR,V_MONEDAID,CENTRO_DE_COSTOId,PLACA,ESTADO_PRESTADO")] ACTIVO aCTIVO)
        {

            var original = db.ACTIVOS.Find(aCTIVO.ID);

            if (original != null)
            {
                original.INICIO_SERVICIO = aCTIVO.INICIO_SERVICIO;
                original.ESTADO_ACTIVOID = aCTIVO.ESTADO_ACTIVOID;
                // si no se cambio el comentario, dejar el original
                original.COMENTARIO = aCTIVO.COMENTARIO.Equals("") ? original.COMENTARIO : aCTIVO.COMENTARIO;
                original.V_USUARIOSIDUSUARIO = aCTIVO.V_USUARIOSIDUSUARIO;
                original.V_ESTACIONID = aCTIVO.V_ESTACIONID;
                original.CENTRO_DE_COSTOId = aCTIVO.CENTRO_DE_COSTOId;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.V_USUARIOSIDUSUARIO = new SelectList(db.V_USUARIOS, "IDUSUARIO", "NOMBRE", aCTIVO.V_USUARIOSIDUSUARIO);
            ViewBag.ESTADO_ACTIVOID = new SelectList(db.ESTADOS_ACTIVOS, "ID", "NOMBRE", aCTIVO.ESTADO_ACTIVOID);
            ViewBag.V_ESTACIONID = new SelectList(db.V_ESTACION, "ID", "NOMBRE", aCTIVO.V_ESTACIONID);
            ViewBag.CENTRO_DE_COSTOId = new SelectList(db.CENTROS_DE_COSTOS, "ID", "NOMBRE", aCTIVO.CENTRO_DE_COSTOId);
            return View(aCTIVO);
        }

        // GET: Activos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTIVO aCTIVO = db.ACTIVOS.Find(id);
            if (aCTIVO == null)
            {
                return HttpNotFound();
            }
            ViewBag.TIPO_TRANSACCIONID = new SelectList(db.TIPOS_TRANSACCIONES, "ID", "NOMBRE", aCTIVO.TIPO_TRANSACCIONID);
            ViewBag.TIPO_ACTIVOID = new SelectList(db.TIPOS_ACTIVOS, "ID", "NOMBRE", aCTIVO.TIPO_ACTIVOID);
            ViewBag.V_PROVEEDORIDPROVEEDOR = new SelectList(db.V_PROVEEDOR, "IDPROVEEDOR", "NOMBRE", aCTIVO.V_PROVEEDORIDPROVEEDOR);
            ViewBag.V_ANFITRIONAID = new SelectList(db.V_ANFITRIONA, "ID", "NOMBRE", aCTIVO.V_ANFITRIONAID);
            ViewBag.V_MONEDAID = new SelectList(db.V_MONEDA, "ID", "SIMBOLO", aCTIVO.V_MONEDAID);
            return View(aCTIVO);
        }

        // POST: Activos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NUMERO_SERIE,FECHA_COMPRA,INICIO_SERVICIO,FECHA_INGRESO,FABRICANTE,PRECIO,DESCRIPCION,EXENTO,PRESTABLE,TIPO_CAPITAL,INGRESADO_POR,NUMERO_DOCUMENTO,NUMERO_LOTE,TIPO_TRANSACCIONID,ESTADO_ACTIVOID,TIPO_ACTIVOID,COMENTARIO,DESECHADO,MODELO,V_USUARIOSIDUSUARIO,V_ESTACIONID,V_ANFITRIONAID,V_PROVEEDORIDPROVEEDOR,V_MONEDAID,CENTRO_DE_COSTOId,PLACA,ESTADO_PRESTADO")] ACTIVO aCTIVO)
        {
            var original = db.ACTIVOS.Find(aCTIVO.ID);
            if (ModelState.IsValid)
            {
                original.NUMERO_SERIE = aCTIVO.NUMERO_SERIE;
                original.FECHA_COMPRA = aCTIVO.FECHA_COMPRA;
                original.INICIO_SERVICIO = aCTIVO.INICIO_SERVICIO;
                original.FECHA_INGRESO = aCTIVO.FECHA_INGRESO;
                original.FABRICANTE = aCTIVO.FABRICANTE;
                original.PRECIO = aCTIVO.PRECIO;
                original.DESCRIPCION = aCTIVO.DESCRIPCION;
                original.EXENTO = aCTIVO.EXENTO;
                original.PRESTABLE = aCTIVO.PRESTABLE;
                original.TIPO_CAPITAL = aCTIVO.TIPO_CAPITAL;
                original.INGRESADO_POR = aCTIVO.INGRESADO_POR;
                original.NUMERO_DOCUMENTO = aCTIVO.NUMERO_DOCUMENTO;
                original.NUMERO_LOTE = aCTIVO.NUMERO_LOTE;
                original.TIPO_TRANSACCIONID = aCTIVO.TIPO_TRANSACCIONID;
                original.TIPO_ACTIVOID = aCTIVO.TIPO_ACTIVOID;
                original.V_ANFITRIONAID = aCTIVO.V_ANFITRIONAID;
                original.V_PROVEEDORIDPROVEEDOR = aCTIVO.V_PROVEEDORIDPROVEEDOR;
                original.DESECHADO = aCTIVO.DESECHADO;
                original.MODELO = aCTIVO.MODELO;
                original.PLACA = aCTIVO.PLACA;
                original.ESTADO_PRESTADO = aCTIVO.ESTADO_PRESTADO;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TIPO_TRANSACCIONID = new SelectList(db.TIPOS_TRANSACCIONES, "ID", "NOMBRE", aCTIVO.TIPO_TRANSACCIONID);
            ViewBag.TIPO_ACTIVOID = new SelectList(db.TIPOS_ACTIVOS, "ID", "NOMBRE", aCTIVO.TIPO_ACTIVOID);
            ViewBag.V_PROVEEDORIDPROVEEDOR = new SelectList(db.V_PROVEEDOR, "IDPROVEEDOR", "NOMBRE", aCTIVO.V_PROVEEDORIDPROVEEDOR);
            ViewBag.V_ANFITRIONAID = new SelectList(db.V_ANFITRIONA, "ID", "NOMBRE", aCTIVO.V_ANFITRIONAID);
            ViewBag.V_MONEDAID = new SelectList(db.V_MONEDA, "ID", "SIMBOLO", aCTIVO.V_MONEDAID);

            return View(aCTIVO);
        }

        // GET: Activos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTIVO aCTIVO = db.ACTIVOS.Find(id);
            if (aCTIVO == null)
            {
                return HttpNotFound();
            }
            return View(aCTIVO);
        }

        // POST: Activos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ACTIVO aCTIVO = db.ACTIVOS.Find(id);
            /*
            aCTIVO.DESECHADO = true;
            var estado = db.ESTADOS_ACTIVOS.ToList().Where(ea => ea.NOMBRE == "Desechado");
            aCTIVO.ESTADO_ACTIVOID = estado.ToList()[0].ID;
            */
            db.ACTIVOS.Remove(aCTIVO); // Quitar esta linea cuando se cambie el estado por desechado
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
    }
}
