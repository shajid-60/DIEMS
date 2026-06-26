-- ============================================================
-- DIEMS — Trigger: ALERT_THRESHOLD_TRG
-- Module: Emergency Resource Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Monitors resource levels after an update and logs a system-level
--   alert in the AUDIT_LOG if availability drops below the category's
--   critical threshold percentage.
-- ============================================================

CREATE OR REPLACE TRIGGER ALERT_THRESHOLD_TRG
AFTER UPDATE OF AVAILABLE_QUANTITY ON RESOURCES
FOR EACH ROW
DECLARE
    v_threshold      NUMBER;
    v_new_pct        NUMBER;
    v_category_name  VARCHAR2(100);
BEGIN
    -- Get threshold percentage and category name
    SELECT CRITICAL_THRESHOLD, CATEGORY_NAME
    INTO   v_threshold, v_category_name
    FROM   RESOURCE_CATEGORIES
    WHERE  CATEGORY_ID = :NEW.CATEGORY_ID;

    -- Calculate stock percentage
    IF :NEW.TOTAL_QUANTITY > 0 THEN
        v_new_pct := (:NEW.AVAILABLE_QUANTITY / :NEW.TOTAL_QUANTITY) * 100;

        -- If stock drops below threshold, log critical alert
        IF v_new_pct <= v_threshold THEN
            INSERT INTO AUDIT_LOG (
                TABLE_NAME, RECORD_ID, OPERATION, COLUMN_NAME,
                OLD_VALUE, NEW_VALUE, CHANGED_BY, MODULE, NOTES
            ) VALUES (
                'RESOURCES', :NEW.RESOURCE_ID, 'UPDATE', 'AVAILABLE_QUANTITY',
                'Stock: ' || ROUND((:OLD.AVAILABLE_QUANTITY / :OLD.TOTAL_QUANTITY) * 100, 1) || '%',
                'ALERT: Stock at ' || ROUND(v_new_pct, 1) || '% (Threshold: ' || v_threshold || '%)',
                'SYSTEM', 'RESOURCE_ALERT',
                'CRITICAL: ' || :NEW.RESOURCE_NAME || ' (' || v_category_name || ') has low stock.'
            );
        END IF;
    END IF;
END;
/
