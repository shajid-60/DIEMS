-- ============================================================
-- DIEMS — Trigger: INVENTORY_UPDATE_TRG
-- Module: Emergency Resource Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Automatically deducts the distributed quantity from the
--   RESOURCES table whenever a new distribution is recorded.
--   Enforces database-level inventory integrity.
-- ============================================================

CREATE OR REPLACE TRIGGER INVENTORY_UPDATE_TRG
AFTER INSERT ON RESOURCE_DISTRIBUTION
FOR EACH ROW
BEGIN
    UPDATE RESOURCES
    SET    AVAILABLE_QUANTITY = AVAILABLE_QUANTITY - :NEW.QUANTITY,
           LAST_UPDATED       = SYSTIMESTAMP,
           UPDATED_BY         = :NEW.DISTRIBUTED_BY
    WHERE  RESOURCE_ID = :NEW.RESOURCE_ID;
END;
/
