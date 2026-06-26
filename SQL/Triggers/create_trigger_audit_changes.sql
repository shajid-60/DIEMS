-- ============================================================
-- DIEMS — Trigger: AUDIT_CHANGES_TRG
-- Module: Disaster Database / Audit
-- Oracle 24.3.1
--
-- PURPOSE:
--   Automatically logs changes to critical fields in the DISASTERS
--   table (like STATUS, CASUALTIES, DISPLACED, and ESTIMATED_DAMAGE)
--   into the central AUDIT_LOG table.
-- ============================================================

CREATE OR REPLACE TRIGGER AUDIT_CHANGES_TRG
AFTER UPDATE ON DISASTERS
FOR EACH ROW
DECLARE
    v_user VARCHAR2(100);
BEGIN
    v_user := USER;

    -- Track Status Changes
    IF (:OLD.STATUS != :NEW.STATUS) THEN
        INSERT INTO AUDIT_LOG (
            TABLE_NAME, RECORD_ID, OPERATION, COLUMN_NAME,
            OLD_VALUE, NEW_VALUE, CHANGED_BY, MODULE
        ) VALUES (
            'DISASTERS', :NEW.DISASTER_ID, 'UPDATE', 'STATUS',
            :OLD.STATUS, :NEW.STATUS, v_user, 'DISASTER_MODULE'
        );
    END IF;

    -- Track Casualties Changes
    IF (:OLD.CASUALTIES != :NEW.CASUALTIES) THEN
        INSERT INTO AUDIT_LOG (
            TABLE_NAME, RECORD_ID, OPERATION, COLUMN_NAME,
            OLD_VALUE, NEW_VALUE, CHANGED_BY, MODULE
        ) VALUES (
            'DISASTERS', :NEW.DISASTER_ID, 'UPDATE', 'CASUALTIES',
            TO_CHAR(:OLD.CASUALTIES), TO_CHAR(:NEW.CASUALTIES), v_user, 'DISASTER_MODULE'
        );
    END IF;

    -- Track Displaced Changes
    IF (:OLD.DISPLACED != :NEW.DISPLACED) THEN
        INSERT INTO AUDIT_LOG (
            TABLE_NAME, RECORD_ID, OPERATION, COLUMN_NAME,
            OLD_VALUE, NEW_VALUE, CHANGED_BY, MODULE
        ) VALUES (
            'DISASTERS', :NEW.DISASTER_ID, 'UPDATE', 'DISPLACED',
            TO_CHAR(:OLD.DISPLACED), TO_CHAR(:NEW.DISPLACED), v_user, 'DISASTER_MODULE'
        );
    END IF;

    -- Track Estimated Damage Changes
    IF (:OLD.ESTIMATED_DAMAGE != :NEW.ESTIMATED_DAMAGE) THEN
        INSERT INTO AUDIT_LOG (
            TABLE_NAME, RECORD_ID, OPERATION, COLUMN_NAME,
            OLD_VALUE, NEW_VALUE, CHANGED_BY, MODULE
        ) VALUES (
            'DISASTERS', :NEW.DISASTER_ID, 'UPDATE', 'ESTIMATED_DAMAGE',
            TO_CHAR(:OLD.ESTIMATED_DAMAGE), TO_CHAR(:NEW.ESTIMATED_DAMAGE), v_user, 'DISASTER_MODULE'
        );
    END IF;
END;
/
