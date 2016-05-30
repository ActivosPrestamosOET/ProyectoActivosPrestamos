------------------------------------
-- Vista de Usuarios desde activos
------------------------------------
 GRANT SELECT ANY TABLE TO "ACTIVOS" WITH ADMIN OPTION;
 create or replace view activos.v_usuarios as
 select * from RESERVAS.usuarios;
-----------------------------------------------------------------------------------
-- Vista del documento de tipo de cambio (para conversiones de dolares a colones)
-----------------------------------------------------------------------------------
-- GRANT SELECT ANY TABLE TO "ACTIVOS" WITH ADMIN OPTION;
 create or replace view activos.v_tipo_cambio as
 select * from FINANCIERO.DOCUMENTO_TIPOCAMBIO;

CREATE SEQUENCE INSERT_TIPO_ACTIVO
  START WITH 1
  INCREMENT BY 1
  NOCACHE
  NOCYCLE;
/

CREATE OR REPLACE TRIGGER id_tipos_activos
  BEFORE INSERT ON TIPOS_ACTIVOS
  FOR EACH ROW
BEGIN
  SELECT INSERT_TIPO_ACTIVO.nextval INTO :new.Id from dual;
END;
/
CREATE SEQUENCE INSERT_TIPO_TRANSACCION
  START WITH 1
  INCREMENT BY 1
  NOCACHE
  NOCYCLE;
/
CREATE OR REPLACE TRIGGER id_tipo_transaccion
    BEFORE INSERT ON tipos_transacciones
    FOR EACH ROW
  BEGIN
    SELECT INSERT_TIPO_TRANSACCION.nextval INTO :new.ID from dual;
  END;
  /

  CREATE SEQUENCE INSERT_ESTADO_ACTIVO
  START WITH 1
  INCREMENT BY 1
  NOCACHE
  NOCYCLE;
/

CREATE OR REPLACE TRIGGER id_estado_activo
    BEFORE INSERT ON estados_activos
    FOR EACH ROW
  BEGIN
    SELECT INSERT_ESTADO_ACTIVO.nextval INTO :new.ID from dual;
  END;
  /

  CREATE SEQUENCE INSERT_TRANSACCION
  START WITH 1
  INCREMENT BY 1
  NOCACHE
  NOCYCLE;
  /

  CREATE OR REPLACE TRIGGER id_transaccion
    BEFORE INSERT ON transacciones
    FOR EACH ROW
  BEGIN
    SELECT INSERT_TRANSACCION.nextval INTO :new.ID from dual;
  END;
  /

  CREATE SEQUENCE INSERT_CENTRO_COSTOS
  START WITH 1
  INCREMENT BY 1
  NOCACHE
  NOCYCLE;
  /

  CREATE OR REPLACE TRIGGER ID_CENTRO_COSTOS
    BEFORE INSERT ON centros_de_costos
    FOR EACH ROW
  BEGIN
    SELECT INSERT_CENTRO_COSTOS.nextval INTO :new."Id" from dual;
  END;
  /

  commit;
