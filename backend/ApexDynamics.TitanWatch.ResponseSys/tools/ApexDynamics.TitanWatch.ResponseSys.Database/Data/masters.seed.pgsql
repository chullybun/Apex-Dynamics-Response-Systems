-- Master/transactional seed rows for the initial world state (re-runnable / idempotent).
-- Applied by `dotnet run -- Data` on every cycle; ON CONFLICT DO NOTHING keeps existing rows
-- (e.g. simulation-mutated state) intact on re-seed.
--   - Leviathans:        the 4-strong ROSTER (src/mock/leviathans.ts).
--   - Dispatch units:    INITIAL_DISPATCH (src/state/useCommandState.ts).
--   - Last-Stand cities: LAST_STAND_CITIES (src/mock/lastStandCities.ts).
-- Reference-data relationships are stored by Code (status_code, threat_code).
-- Numeric stats (range/height/speed/hp/hp_max/heading) are seeded within the mock's
-- generation ranges; the Phase 5 simulation engine drives their live values.

INSERT INTO "responsesys"."leviathan"
  ("leviathan_id", "codename", "class_numeral", "archetype", "range", "height", "speed", "hp", "hp_max", "status_code", "lng", "lat", "threat_code", "heading", "from_lng", "from_lat", "to_lng", "to_lat", "target", "start_range", "repel")
VALUES
  ('lev-1', 'Gorathos', 'V',   'Abyssal Colossus',  12.5, 168, 47.0, 6050, 11000, 'LND', -122.470, 47.660, 'CAT', 128, -122.470, 47.660, -122.333, 47.606, 'SEATTLE',       215, 0),
  ('lev-2', 'Vespyra',  'IV',  'Tempest Wyrm',      22.4, 140, 63.0, 5850,  9000, 'INB', -122.540, 47.560, 'CRI',  90, -122.540, 47.560, -122.200, 47.610, 'BELLEVUE',       91, 0),
  ('lev-3', 'Terrakon', 'III', 'Tectonic Behemoth', 35.0, 155, 40.0, 6000,  8000, 'SUR', -122.420, 47.540, 'ELV', 210, -122.420, 47.540, -122.225, 47.585, 'MERCER ISLAND', 165, 0),
  ('lev-4', 'Nyxmora',  'II',  'Umbral Leviathan',  48.0,  96, 28.0, 5950,  7000, 'SUB', -122.320, 47.730, 'STR', 300, -122.320, 47.730, -122.210, 47.678, 'KIRKLAND',      188, 0)
ON CONFLICT ("leviathan_id") DO NOTHING;

INSERT INTO "responsesys"."dispatch_unit"
  ("dispatch_unit_id", "name", "available", "capacity")
VALUES
  ('scramble-jets', 'Scramble Jets', 6, 6),
  ('deploy-mechs',  'Deploy Mechs',  4, 4),
  ('raise-barrier', 'Raise Barrier', 3, 3),
  ('evac-sector',   'Evac Sector',   8, 8)
ON CONFLICT ("dispatch_unit_id") DO NOTHING;

INSERT INTO "responsesys"."last_stand_city"
  ("last_stand_city_id", "name", "side", "population")
VALUES
  ('bremerton', 'BREMERTON', 'left',  412000),
  ('olympia',   'OLYMPIA',   'right', 318000)
ON CONFLICT ("last_stand_city_id") DO NOTHING;
