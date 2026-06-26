-- ============================================================
-- DIEMS — Function: CALCULATE_DAMAGE
-- Module: Disaster Database / Analytics
-- Oracle 24.3.1
--
-- PURPOSE:
--   Returns the total estimated financial damage (in BDT)
--   for a given disaster. Optionally calculates weighted
--   damage based on casualties and displaced population.
--
-- PARAMETERS:
--   p_disaster_id   IN NUMBER  - Disaster to calculate for
--
-- RETURNS: NUMBER - Total damage in BDT (Taka)
--          Returns 0 if disaster not found
--
-- USAGE:
--   SELECT CALCULATE_DAMAGE(1) FROM DUAL;
--   SELECT DISASTER_NAME, CALCULATE_DAMAGE(DISASTER_ID) AS DAMAGE
--   FROM DISASTERS WHERE STATUS = 'ACTIVE';
-- ============================================================

CREATE OR REPLACE FUNCTION CALCULATE_DAMAGE (
    p_disaster_id IN NUMBER
)
RETURN NUMBER
AS
    v_base_damage        NUMBER := 0;
    v_infrastructure     NUMBER := 0;
    v_human_cost         NUMBER := 0;
    v_total_damage       NUMBER := 0;
    v_casualties         NUMBER := 0;
    v_displaced          NUMBER := 0;
    v_affected_areas     NUMBER := 0;

    -- Constants for damage estimation (in BDT)
    c_cost_per_casualty  CONSTANT NUMBER := 5000000;    -- 50 Lakh per casualty
    c_cost_per_displaced CONSTANT NUMBER := 15000;      -- 15,000 per displaced person
    c_cost_per_km2       CONSTANT NUMBER := 2500000;    -- 25 Lakh per km² infrastructure

BEGIN
    -- Step 1: Get base damage from DISASTERS table
    SELECT NVL(ESTIMATED_DAMAGE, 0),
           NVL(CASUALTIES, 0),
           NVL(DISPLACED, 0)
    INTO   v_base_damage,
           v_casualties,
           v_displaced
    FROM   DISASTERS
    WHERE  DISASTER_ID = p_disaster_id;

    -- Step 2: Calculate affected area damage
    SELECT NVL(SUM(AREA_KM2), 0)
    INTO   v_affected_areas
    FROM   AFFECTED_AREAS
    WHERE  DISASTER_ID = p_disaster_id;

    -- Step 3: Calculate human cost component
    v_human_cost := (v_casualties * c_cost_per_casualty)
                  + (v_displaced  * c_cost_per_displaced);

    -- Step 4: Calculate infrastructure damage
    v_infrastructure := v_affected_areas * c_cost_per_km2;

    -- Step 5: Total = Base + Human Cost + Infrastructure
    -- Use base if provided; otherwise estimate from components
    IF v_base_damage > 0 THEN
        v_total_damage := v_base_damage + v_human_cost;
    ELSE
        v_total_damage := v_human_cost + v_infrastructure;
    END IF;

    RETURN v_total_damage;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 0;
    WHEN OTHERS THEN
        RETURN -1;  -- Error indicator
END CALCULATE_DAMAGE;
/

-- Example usage:
-- SELECT DISASTER_NAME,
--        CALCULATE_DAMAGE(DISASTER_ID)           AS TOTAL_DAMAGE_BDT,
--        ROUND(CALCULATE_DAMAGE(DISASTER_ID)/10000000, 2) AS DAMAGE_IN_CRORE
-- FROM   DISASTERS
-- WHERE  STATUS = 'ACTIVE'
-- ORDER BY TOTAL_DAMAGE_BDT DESC;
