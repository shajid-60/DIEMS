-- ============================================================
-- DIEMS — Trigger: SHELTER_CAP_VAL_TRG
-- Module: Shelter Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Validates shelter capacity before inserting a new resident.
--   Prevents over-allocation unless overflow protocol is active.
-- ============================================================

CREATE OR REPLACE TRIGGER SHELTER_CAP_VAL_TRG
BEFORE INSERT ON SHELTER_RESIDENTS
FOR EACH ROW
DECLARE
    v_available NUMBER;
    v_overflow  NUMBER;
BEGIN
    SELECT AVAILABLE_BEDS, HAS_OVERFLOW
    INTO   v_available, v_overflow
    FROM   SHELTER_CAPACITY
    WHERE  SHELTER_ID = :NEW.SHELTER_ID;

    IF v_available <= 0 AND v_overflow = 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'SHELTER_ERROR: Shelter capacity exceeded. No available beds.');
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        -- If no capacity tracking record exists, allow entry
        NULL;
END;
/
