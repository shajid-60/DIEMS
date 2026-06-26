-- ============================================================
-- DIEMS — Database Schema Installation Script
-- Oracle 11g Compatibility
-- ============================================================

SET SERVEROUTPUT ON;
SET FEEDBACK ON;
SET DEFINE OFF;

-- Automatically change directory to the SQL folder
CD "C:\Users\MAJHARUL SHAJID\Desktop\DIEMS-ASP\SQL"

PROMPT Cleaning up existing database objects...
@@drop_all.sql

PROMPT ========================================================
PROMPT   STARTING DIEMS DATABASE SCHEMA INSTALLATION
PROMPT ========================================================

PROMPT [1/7] Creating Tables...
@@01_create_tables.sql

PROMPT [2/7] Checking Sequences...
@@02_create_sequences.sql

PROMPT [3/7] Creating Views...
@@03_create_views.sql

PROMPT [4/7] Creating Procedures...
@@04_create_procedures.sql

PROMPT [5/7] Creating Functions...
@@05_create_functions.sql

PROMPT [6/7] Creating Triggers...
@@06_create_triggers.sql

PROMPT [6.5/7] Seeding Lookup Data...
@@06_seed_lookup_data.sql

PROMPT [7/7] Seeding Sample Data...
@@07_sample_data.sql

PROMPT ========================================================
PROMPT   DIEMS DATABASE INSTALLED SUCCESSFULLY!
PROMPT ========================================================
