﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Activos_PrestamosOET.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PrestamosEntities : DbContext
    {
        public PrestamosEntities()
            : base("name=PrestamosEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<EQUIPO_SOLICITADO> EQUIPO_SOLICITADO { get; set; }
        public virtual DbSet<PRESTAMO> PRESTAMOS { get; set; }
        public virtual DbSet<ACTIVO> ACTIVOS { get; set; }
        public virtual DbSet<USUARIO> USUARIOS { get; set; }
    }
}
