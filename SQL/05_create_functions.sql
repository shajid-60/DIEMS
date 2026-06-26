-- ============================================================
-- DIEMS — 05_create_functions.sql
-- Module: Master Functions Builder
-- Oracle 24.3.1
-- ============================================================

PROMPT Creating CALCULATE_DAMAGE function...
@@Functions/create_function_calculate_damage.sql

PROMPT Creating AVAILABLE_RESOURCES function...
@@Functions/create_function_available_resources.sql

PROMPT Creating TOTAL_VICTIMS function...
@@Functions/create_function_total_victims.sql

PROMPT All functions created successfully!
