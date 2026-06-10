CREATE OR REPLACE TRIGGER trg_disasters_bi
BEFORE INSERT ON DISASTERS
FOR EACH ROW
BEGIN
    IF :NEW.disaster_id IS NULL THEN
        SELECT seq_disasters.NEXTVAL
        INTO :NEW.disaster_id
        FROM dual;
    END IF;
END;
/