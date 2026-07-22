-- Create transactional table: "responsesys"."last_stand_city"
-- A city in the Last-Stand scenario (src/mock/lastStandCities.ts LastStandCity).

BEGIN TRANSACTION;

CREATE TABLE "responsesys"."last_stand_city" (
  "last_stand_city_id" VARCHAR(50)  NOT NULL PRIMARY KEY,
  "name"               VARCHAR(100) NOT NULL,
  "side"               VARCHAR(10)  NULL,     -- field side: 'left' | 'right'
  "population"         INTEGER      NULL,
  "created_by"         VARCHAR(250) NULL,
  "created_on"         TIMESTAMPTZ  NULL,
  "updated_by"         VARCHAR(250) NULL,
  "updated_on"         TIMESTAMPTZ  NULL
);

COMMIT TRANSACTION;
