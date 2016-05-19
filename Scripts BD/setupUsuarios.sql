CREATE TABLE "ActivosRoles" (
  "Id" NVARCHAR2(128) NOT NULL,
  "Name" NVARCHAR2(256) NOT NULL,
  PRIMARY KEY ("Id")
);


CREATE TABLE "ActivosUserRoles" (
  "UserId" NVARCHAR2(128) NOT NULL,
  "RoleId" NVARCHAR2(128) NOT NULL,
  PRIMARY KEY ("UserId", "RoleId")
);


CREATE TABLE "ActivosUsers" (
  "Id" NVARCHAR2(128) NOT NULL,
  "Email" NVARCHAR2(256) NULL,
  "EmailConfirmed" NUMBER(1) NOT NULL,
  "PasswordHash" NVARCHAR2(256) NULL,
  "SecurityStamp" NVARCHAR2(256) NULL,
  "PhoneNumber" NVARCHAR2(256) NULL,
  "PhoneNumberConfirmed" NUMBER(1) NOT NULL,
  "TwoFactorEnabled" NUMBER(1) NOT NULL,
  "LockoutEndDateUtc" TIMESTAMP(7) NULL,
  "LockoutEnabled" NUMBER(1) NOT NULL,
  "AccessFailedCount" NUMBER(10) NOT NULL,
  "UserName" NVARCHAR2(256) NOT NULL,
  PRIMARY KEY ("Id")
);


CREATE TABLE "ActivosUserClaims" (
  "Id" NUMBER(10) NOT NULL,
  "UserId" NVARCHAR2(128) NOT NULL,
  "ClaimType" NVARCHAR2(256) NULL,
  "ClaimValue" NVARCHAR2(256) NULL,
  PRIMARY KEY ("Id")
);


CREATE SEQUENCE "ActivosUserClaims_SEQ";

CREATE TABLE "ActivosUserLogins" (
  "LoginProvider" NVARCHAR2(128) NOT NULL,
  "ProviderKey" NVARCHAR2(128) NOT NULL,
  "UserId" NVARCHAR2(128) NOT NULL,
  PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId")
);


CREATE UNIQUE INDEX "RoleNameIndex" ON "ActivosRoles" ("Name");

CREATE INDEX "IX_ActivosUserRoles_UserId" ON "ActivosUserRoles" ("UserId");


CREATE INDEX "IX_ActivosUserRoles_RoleId" ON "ActivosUserRoles" ("RoleId");


CREATE UNIQUE INDEX "UserNameIndex" ON "ActivosUsers" ("UserName");


CREATE INDEX "IX_ActivosUserClaims_UserId" ON "ActivosUserClaims" ("UserId");


CREATE INDEX "IX_ActivosUserLogins_UserId" ON "ActivosUserLogins" ("UserId");


ALTER TABLE "ActivosUserRoles"
  ADD CONSTRAINT "FK_UserRoles_Roles" FOREIGN KEY ("RoleId") REFERENCES "ActivosRoles" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "ActivosUserRoles"
  ADD CONSTRAINT "FK_UserRoles_Users" FOREIGN KEY ("UserId") REFERENCES "ActivosUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "ActivosUserClaims"
  ADD CONSTRAINT "FK_UserClaims_Users" FOREIGN KEY ("UserId") REFERENCES "ActivosUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "ActivosUserLogins"
  ADD CONSTRAINT "FK_UserLogins_Users" FOREIGN KEY ("UserId") REFERENCES "ActivosUsers" ("Id")
  ON DELETE CASCADE;

  CREATE OR REPLACE TRIGGER "ActivosUserClaims_INS_TRG"
    BEFORE INSERT ON "ActivosUserClaims"
    FOR EACH ROW
  BEGIN
    SELECT "ActivosUserClaims_SEQ".NEXTVAL INTO :NEW."Id" FROM DUAL;
  END;
