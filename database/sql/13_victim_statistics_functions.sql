CREATE OR REPLACE FUNCTION total_victims
(
    p_disaster_id IN NUMBER
)
RETURN NUMBER
IS
    v_total NUMBER;
BEGIN

    SELECT COUNT(*)
    INTO v_total
    FROM victims
    WHERE disaster_id = p_disaster_id;

    RETURN v_total;

END;
/

CREATE OR REPLACE FUNCTION total_injured
(
    p_disaster_id IN NUMBER
)
RETURN NUMBER
IS
    v_total NUMBER;
BEGIN

    SELECT COUNT(*)
    INTO v_total
    FROM victims
    WHERE disaster_id = p_disaster_id
      AND status = 'INJURED';

    RETURN v_total;

END;
/

CREATE OR REPLACE FUNCTION total_missing
(
    p_disaster_id IN NUMBER
)
RETURN NUMBER
IS
    v_total NUMBER;
BEGIN

    SELECT COUNT(*)
    INTO v_total
    FROM victims
    WHERE disaster_id = p_disaster_id
      AND status = 'MISSING';

    RETURN v_total;

END;
/