-- ============================================================
-- DIEMS — Trigger: SHELTER_CAP_UPD_TRG
-- Module: Shelter Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Automatically updates the occupied counts and available beds in
--   SHELTER_CAPACITY when a resident is checked in or checked out.
-- ============================================================

CREATE OR REPLACE TRIGGER SHELTER_CAP_UPD_TRG
AFTER INSERT OR UPDATE ON SHELTER_RESIDENTS
FOR EACH ROW
BEGIN
    -- Check-in: Increment occupied count
    IF INSERTING AND :NEW.STATUS = 'Active' THEN
        UPDATE SHELTER_CAPACITY
        SET    CURRENT_OCCUPIED = CURRENT_OCCUPIED + 1,
               AVAILABLE_BEDS   = MAX_CAPACITY - (CURRENT_OCCUPIED + 1) - RESERVED_SPOTS,
               LAST_UPDATED     = SYSTIMESTAMP
        WHERE  SHELTER_ID = :NEW.SHELTER_ID;
    END IF;

    -- Check-out/Discharge: Decrement occupied count
    IF UPDATING AND :OLD.STATUS = 'Active' AND :NEW.STATUS != 'Active' THEN
        UPDATE SHELTER_CAPACITY
        SET    CURRENT_OCCUPIED = CURRENT_OCCUPIED - 1,
               AVAILABLE_BEDS   = MAX_CAPACITY - (CURRENT_OCCUPIED - 1) - RESERVED_SPOTS,
               LAST_UPDATED     = SYSTIMESTAMP
        WHERE  SHELTER_ID = :NEW.SHELTER_ID;
    END IF;
END;
/
