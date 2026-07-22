-- Create transactional table: "responsesys"."leviathan"
-- A tracked apex threat. Mirrors the front-end Leviathan shape (src/mock/types.ts):
-- reference-data relationships (status, threat) are stored by Code (no FK, by convention);
-- the from/to inbound-track coordinates are flattened to from_lng/from_lat/to_lng/to_lat.
-- PostgreSQL uses the hidden xmin system column for optimistic concurrency (ETag) -- no RowVersion column.

BEGIN TRANSACTION;

CREATE TABLE "responsesys"."leviathan" (
  "leviathan_id"  VARCHAR(50)      NOT NULL PRIMARY KEY,
  "codename"      VARCHAR(100)     NOT NULL,
  "class_numeral" VARCHAR(10)      NULL,
  "archetype"     VARCHAR(100)     NULL,
  "range"         DOUBLE PRECISION NULL,
  "height"        INTEGER          NULL,
  "speed"         DOUBLE PRECISION NULL,
  "hp"            INTEGER          NULL,
  "hp_max"        INTEGER          NULL,
  "status_code"   VARCHAR(50)      NULL,   -- references "leviathan_status"."code"; no FK by convention
  "lng"           DOUBLE PRECISION NULL,
  "lat"           DOUBLE PRECISION NULL,
  "threat_code"   VARCHAR(50)      NULL,   -- references "threat_level"."code"; no FK by convention
  "heading"       DOUBLE PRECISION NULL,
  "from_lng"      DOUBLE PRECISION NULL,
  "from_lat"      DOUBLE PRECISION NULL,
  "to_lng"        DOUBLE PRECISION NULL,
  "to_lat"        DOUBLE PRECISION NULL,
  "target"        VARCHAR(100)     NULL,
  "start_range"   INTEGER          NULL,
  "repel"         DOUBLE PRECISION NULL,
  "is_deleted"    BOOLEAN          NOT NULL DEFAULT FALSE,   -- logical (soft) delete; no contract property
  "created_by"    VARCHAR(250)     NULL,
  "created_on"    TIMESTAMPTZ      NULL,
  "updated_by"    VARCHAR(250)     NULL,
  "updated_on"    TIMESTAMPTZ      NULL
);

COMMIT TRANSACTION;
