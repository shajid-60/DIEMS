-- ============================================================
-- DIEMS — 03_create_views.sql
-- Module: Master Views Builder
-- Oracle 24.3.1
-- ============================================================

PROMPT Creating ACTIVE_DISASTERS_VW...
@@Views/create_view_active_disasters.sql

PROMPT Creating AVAILABLE_SHELTERS_VW...
@@Views/create_view_available_shelters.sql

PROMPT Creating CRITICAL_RESOURCES_VW...
@@Views/create_view_critical_resources.sql

PROMPT All views created successfully!
