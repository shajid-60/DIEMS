/* =====================================
   SHELTERS AUTO-ID TRIGGER
===================================== */

CREATE OR REPLACE TRIGGER trg_shelters_bi
BEFORE INSERT ON shelters
FOR EACH ROW
BEGIN
    IF :NEW.shelter_id IS NULL THEN
        SELECT seq_shelters.NEXTVAL
        INTO :NEW.shelter_id
        FROM dual;
    END IF;
END;
/
/* =====================================
   SHELTER ASSIGNMENTS AUTO-ID TRIGGER
===================================== */

CREATE OR REPLACE TRIGGER trg_shelter_assignments_bi
BEFORE INSERT ON shelter_assignments
FOR EACH ROW
BEGIN
    IF :NEW.assignment_id IS NULL THEN
        SELECT seq_shelter_assignments.NEXTVAL
        INTO :NEW.assignment_id
        FROM dual;
    END IF;
END;
/