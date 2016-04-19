-- --------------------------------------------------
-- Entity Designer DDL Script for Oracle database
-- --------------------------------------------------
-- Date Created: 19/04/2016 12:30:17
-- Generated from EDMX file: C:\Users\Carolina\Documents\GitHub\ProyectoActivosPrestamos\Activos-PrestamosOET\Models\PrestamosOET.edmx
-- --------------------------------------------------

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

-- ALTER TABLE "ACTIVOS"."EQUIPO_SOLICITADO" DROP CONSTRAINT "FK_A_PRESTAMOS" CASCADE;
-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

-- DROP TABLE "ACTIVOS"."EQUIPO_SOLICITADO";

-- DROP TABLE "ACTIVOS"."PRESTAMOS";

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'EQUIPO_SOLICITADO'
CREATE TABLE "ACTIVOS"."EQUIPO_SOLICITADO" (
   "ID_PRESTAMO" VARCHAR2(8 ) NOT NULL,
   "TIPO_ACTIVO" VARCHAR2(50 ) NOT NULL,
   "CANTIDAD" NUMBER(38) NOT NULL
);

-- Creating table 'PRESTAMOS'
CREATE TABLE "ACTIVOS"."PRESTAMOS" (
   "ID" VARCHAR2(8 ) NOT NULL,
   "NUMERO_BOLETA" NUMBER(38) ,
   "MOTIVO" VARCHAR2(250 ) ,
   "FECHA_SOLICITUD" DATE ,
   "FECHA_RETIRO" DATE ,
   "PERIODO_USO" NUMBER(38) ,
   "SOFTWARE_REQUERIDO" VARCHAR2(250 ) ,
   "OBSERVACIONES_SOLICITANTE" VARCHAR2(250 ) ,
   "OBSERVACIONES_APROBADO" VARCHAR2(250 ) ,
   "OBSERVACIONES_RECIBIDO" VARCHAR2(250 ) ,
   "SIGLA_CURSO" CHAR(8 ) ,
   "Estado" NUMBER(5) NOT NULL,
   "CEDULA_SOLICITANTE" NUMBER(10) NOT NULL,
   "CEDULA_APRUEBA" NUMBER(10) NOT NULL
);


-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on "ID_PRESTAMO", "TIPO_ACTIVO", "CANTIDAD"in table 'EQUIPO_SOLICITADO'
ALTER TABLE "ACTIVOS"."EQUIPO_SOLICITADO"
ADD CONSTRAINT "PK_EQUIPO_SOLICITADO"
   PRIMARY KEY ("ID_PRESTAMO", "TIPO_ACTIVO", "CANTIDAD" )
   ENABLE
   VALIDATE;


-- Creating primary key on "ID"in table 'PRESTAMOS'
ALTER TABLE "ACTIVOS"."PRESTAMOS"
ADD CONSTRAINT "PK_PRESTAMOS"
   PRIMARY KEY ("ID" )
   ENABLE
   VALIDATE;


-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on "ID_PRESTAMO" in table 'EQUIPO_SOLICITADO'
ALTER TABLE "ACTIVOS"."EQUIPO_SOLICITADO"
ADD CONSTRAINT "FK_A_PRESTAMOS"
   FOREIGN KEY ("ID_PRESTAMO")
   REFERENCES "ACTIVOS"."PRESTAMOS"
       ("ID")
;

-- Creating index for FOREIGN KEY 'FK_A_PRESTAMOS'
CREATE INDEX "IX_FK_A_PRESTAMOS"
ON "ACTIVOS"."EQUIPO_SOLICITADO"
   ("ID_PRESTAMO");

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
