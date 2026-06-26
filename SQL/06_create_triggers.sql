-- ============================================================
-- DIEMS — 06_create_triggers.sql
-- Module: Master Triggers Builder
-- Oracle 24.3.1
-- ============================================================

PROMPT Creating INVENTORY_UPDATE_TRG trigger...
@@Triggers/create_trigger_inventory_update.sql

PROMPT Creating AUDIT_CHANGES_TRG trigger...
@@Triggers/create_trigger_audit_changes.sql

PROMPT Creating SHELTER_CAP_VAL_TRG trigger...
@@Triggers/create_trigger_shelter_cap_validation.sql

PROMPT Creating SHELTER_CAP_UPD_TRG trigger...
@@Triggers/create_trigger_shelter_cap_update.sql

PROMPT Creating ALERT_THRESHOLD_TRG trigger...
@@Triggers/create_trigger_alert_threshold.sql

PROMPT All triggers created successfully!
