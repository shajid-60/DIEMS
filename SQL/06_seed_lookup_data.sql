-- ============================================================
-- DIEMS — Seed Lookup Data
-- Module: Initial Configuration
-- Oracle 11g Compatibility
-- ============================================================

-- From create_table_disaster_types.sql
-- Seed initial disaster types
INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (1, 'Cyclone',   '🌀', '#AA00FF', 'Tropical cyclone or hurricane making landfall');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (2, 'Flood',     '🌊', '#2979FF', 'River overflow, flash flood or tidal surge');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (3, 'Earthquake','🌍', '#FF8C00', 'Seismic activity causing structural damage');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (4, 'Fire',      '🔥', '#FF3B3B', 'Forest fire, industrial fire or building fire');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (5, 'Landslide', '⛰️', '#8BC34A', 'Hill slope collapse or mudslide');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (6, 'Tornado',   '🌪️', '#E040FB', 'Rotating windstorm with funnel cloud');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (7, 'Tsunami',   '🌊', '#0288D1', 'Ocean wave triggered by seismic event');

INSERT INTO DISASTER_TYPES (TYPE_ID, TYPE_NAME, ICON, COLOR_CODE, DESCRIPTION)
VALUES (8, 'Drought',   '☀️', '#F57F17', 'Prolonged water shortage affecting agriculture');

COMMIT;

-- From create_table_severity_levels.sql
-- Seed severity levels
INSERT INTO SEVERITY_LEVELS (LEVEL_ID, LEVEL_NAME, LEVEL_CODE, COLOR_CODE, DESCRIPTION)
VALUES (1, 'Critical', 4, '#FF3B3B', 'Immediate threat to life - mass casualties possible - full emergency response required');

INSERT INTO SEVERITY_LEVELS (LEVEL_ID, LEVEL_NAME, LEVEL_CODE, COLOR_CODE, DESCRIPTION)
VALUES (2, 'High',     3, '#FF8C00', 'Significant danger - large population affected - major response required');

INSERT INTO SEVERITY_LEVELS (LEVEL_ID, LEVEL_NAME, LEVEL_CODE, COLOR_CODE, DESCRIPTION)
VALUES (3, 'Medium',   2, '#FFD600', 'Moderate impact - localized area - coordinated response needed');

INSERT INTO SEVERITY_LEVELS (LEVEL_ID, LEVEL_NAME, LEVEL_CODE, COLOR_CODE, DESCRIPTION)
VALUES (4, 'Low',      1, '#00E676', 'Minor impact - limited area - standard response sufficient');

COMMIT;

-- From create_table_resource_categories.sql
-- Seed categories
INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (1, 'Food Packets',    'packets',  '🍞', 20, 'Emergency food rations per person per day');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (2, 'Clean Water',     'liters',   '💧', 30, 'Potable water in bottles or containers');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (3, 'Medicine Kits',   'kits',     '💊', 35, 'First aid and emergency medicine kits');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (4, 'Blankets',        'pieces',   '🏠', 15, 'Thermal blankets for shelter');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (5, 'Rescue Boats',    'boats',    '🚤', 30, 'Inflatable and motorized rescue boats');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (6, 'Tents',           'units',    '⛺', 20, 'Emergency shelter tents');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (7, 'Life Jackets',    'pieces',   '🦺', 25, 'Personal flotation devices');

INSERT INTO RESOURCE_CATEGORIES (CATEGORY_ID, CATEGORY_NAME, UNIT, ICON, CRITICAL_THRESHOLD, DESCRIPTION)
VALUES (8, 'Emergency Kits',  'kits',     '🎒', 25, 'Multi-purpose emergency survival kits');

COMMIT;

-- From create_table_skills.sql
-- Seed skill categories
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (1, 'First Aid',               'Medical',    'Basic first aid and CPR certified');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (2, 'Emergency Medicine',      'Medical',    'Advanced emergency medical procedures');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (3, 'Search and Rescue',       'Technical',  'SAR operations in collapsed structures');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (4, 'Boat Operation',          'Technical',  'Licensed boat operator for water rescue');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (5, 'Communication',           'Technical',  'Ham radio, satellite phone operation');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (6, 'Food Distribution',       'Logistics',  'Managing large-scale food distribution');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (7, 'Heavy Lifting',           'Physical',   'Physical assistance in rescue operations');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (8, 'Psychological Support',   'Medical',    'Trauma counseling and mental health');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (9, 'Engineering',             'Technical',  'Structural assessment and debris removal');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (10, 'Translation',             'Language',   'Multi-language communication support');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (11, 'Drone Operation',         'Technical',  'UAV operation for aerial assessment');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (12, 'Logistics Management',    'Logistics',  'Supply chain and warehouse management');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (13, 'Driving/Transport',       'Logistics',  'Heavy vehicle and transport operation');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (14, 'Swimming',                'Physical',   'Strong swimmer for water rescue support');
INSERT INTO SKILLS (SKILL_ID, SKILL_NAME, CATEGORY, DESCRIPTION) VALUES (15, 'IT/Data Entry',           'Technical',  'Data entry and IT support for field operations');

COMMIT;
