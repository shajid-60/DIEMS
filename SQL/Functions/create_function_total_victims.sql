-- ============================================================
-- DIEMS — Function: TOTAL_VICTIMS
-- Module: Victim Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Returns the total number of victims registered for a given
--   disaster.
--
-- PARAMETERS:
--   p_disaster_id   IN NUMBER  - Disaster ID to count victims for
--
-- RETURNS: NUMBER - Total count of registered victims
--
-- USAGE:
--   SELECT TOTAL_VICTIMS(1) FROM DUAL;
-- ============================================================

CREATE OR REPLACE FUNCTION TOTAL_VICTIMS (
    p_disaster_id IN NUMBER
)
RETURN NUMBER
AS
    v_count NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO   v_count
    FROM   VICTIMS
    WHERE  DISASTER_ID = p_disaster_id;

    RETURN v_count;
EXCEPTION
    WHEN OTHERS THEN
        RETURN 0;
END TOTAL_VICTIMS;
/
