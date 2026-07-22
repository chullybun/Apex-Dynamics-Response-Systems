-- Create reference-data table: "responsesys"."threat_level"
-- Threat severity scale (Dormant -> Cataclysm). Includes an extra "color" column
-- mirroring the front-end severity hex values (src/mock/severity.ts).

BEGIN TRANSACTION;

CREATE TABLE "responsesys"."threat_level" (
  "threat_level_id" VARCHAR(50)  NOT NULL PRIMARY KEY,
  "code"            VARCHAR(50)  NOT NULL UNIQUE,
  "text"            VARCHAR(250) NULL,
  "is_active"       BOOLEAN      NULL,
  "sort_order"      INTEGER      NULL,
  "color"           VARCHAR(50)  NULL,          -- hex colour for the threat level (e.g. #10b981)
  "created_by"      VARCHAR(250) NULL,
  "created_on"      TIMESTAMPTZ  NULL,
  "updated_by"      VARCHAR(250) NULL,
  "updated_on"      TIMESTAMPTZ  NULL
);

COMMIT TRANSACTION;
