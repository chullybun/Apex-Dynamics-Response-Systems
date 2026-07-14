-- Create transactional table: "responsesys"."dispatch_unit"
-- A deployable response asset with finite capacity (src/mock/types.ts DispatchUnit).

BEGIN TRANSACTION;

CREATE TABLE "responsesys"."dispatch_unit" (
  "dispatch_unit_id" VARCHAR(50)  NOT NULL PRIMARY KEY,
  "name"             VARCHAR(100) NOT NULL,
  "available"        INTEGER      NULL,
  "capacity"         INTEGER      NULL,
  "created_by"       VARCHAR(250) NULL,
  "created_on"       TIMESTAMPTZ  NULL,
  "updated_by"       VARCHAR(250) NULL,
  "updated_on"       TIMESTAMPTZ  NULL
);

COMMIT TRANSACTION;
