-- Create transactional table: "responsesys"."signal_event"
-- A single timestamped Signal Feed entry. Severity is stored by Code (no FK, by convention).
-- "timestamp" holds epoch-milliseconds to mirror the front-end SignalEvent shape (src/mock/types.ts).

BEGIN TRANSACTION;

CREATE TABLE "responsesys"."signal_event" (
  "signal_event_id" VARCHAR(50)  NOT NULL PRIMARY KEY,
  "severity_code"   VARCHAR(50)  NULL,      -- references "signal_severity"."code"; no FK by convention
  "message"         VARCHAR(500) NULL,
  "timestamp"       BIGINT       NULL,      -- event time (epoch milliseconds)
  "leviathan_id"    VARCHAR(50)  NULL,      -- optional link to the related leviathan
  "created_by"      VARCHAR(250) NULL,
  "created_on"      TIMESTAMPTZ  NULL,
  "updated_by"      VARCHAR(250) NULL,
  "updated_on"      TIMESTAMPTZ  NULL
);

COMMIT TRANSACTION;
