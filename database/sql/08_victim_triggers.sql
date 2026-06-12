/* =====================================
   VICTIMS AUTO-ID TRIGGER
===================================== */

CREATE OR REPLACE TRIGGER trg_victims_bi
BEFORE INSERT ON victims
FOR EACH ROW
BEGIN
    IF :NEW.victim_id IS NULL THEN
        SELECT seq_victims.NEXTVAL
        INTO :NEW.victim_id
        FROM dual;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_missing_persons_bi
BEFORE INSERT ON missing_persons
FOR EACH ROW
BEGIN
    IF :NEW.missing_id IS NULL THEN
        SELECT seq_missing_persons.NEXTVAL
        INTO :NEW.missing_id
        FROM dual;
    END IF;
END;
/