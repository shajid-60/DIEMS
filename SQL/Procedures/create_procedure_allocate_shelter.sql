-- ============================================================
-- DIEMS — Stored Procedure: ALLOCATE_SHELTER
-- Module: Shelter Management / Victim Management
-- Oracle 24.3.1
-- 
-- PURPOSE:
--   Automatically assigns a victim to the nearest available
--   shelter in their district. If the district has no space,
--   expands search to same division.
--
-- PARAMETERS:
--   p_victim_id    IN  NUMBER  - Victim to be sheltered
--   p_disaster_id  IN  NUMBER  - Related disaster (for filtering)
--   p_result       OUT NUMBER  - Assigned SHELTER_ID (0 = none found)
--   p_message      OUT VARCHAR2 - Status message
--
-- CALLED BY:
--   - ASP.NET VictimController.RegisterVictim()
--   - Victim registration form submit
-- ============================================================

CREATE OR REPLACE PROCEDURE ALLOCATE_SHELTER (
    p_victim_id   IN  NUMBER,
    p_disaster_id IN  NUMBER,
    p_result      OUT NUMBER,
    p_message     OUT VARCHAR2
)
AS
    v_shelter_id     NUMBER := 0;
    v_victim_district VARCHAR2(100);
    v_victim_division VARCHAR2(100);
    v_shelter_name   VARCHAR2(200);
    v_available      NUMBER;

BEGIN
    -- Step 1: Get victim's district and division
    SELECT v.DISTRICT
    INTO   v_victim_district
    FROM   VICTIMS v
    WHERE  v.VICTIM_ID = p_victim_id;

    -- Step 2: Get division for the disaster
    SELECT d.DIVISION
    INTO   v_victim_division
    FROM   DISASTERS d
    WHERE  d.DISASTER_ID = p_disaster_id;

    -- Step 3: Find nearest shelter in same district with available beds
    BEGIN
        SELECT SHELTER_ID, SHELTER_NAME, AVAILABLE_BEDS
        INTO   v_shelter_id, v_shelter_name, v_available
        FROM (
            SELECT s.SHELTER_ID,
                   s.SHELTER_NAME,
                   sc.AVAILABLE_BEDS
            FROM   SHELTERS         s
            JOIN   SHELTER_CAPACITY sc ON s.SHELTER_ID = sc.SHELTER_ID
            WHERE  s.DISTRICT   = v_victim_district
              AND  s.IS_ACTIVE   = 1
              AND  sc.AVAILABLE_BEDS > 0
            ORDER BY sc.AVAILABLE_BEDS DESC
        ) WHERE ROWNUM = 1;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            -- Step 4: No district shelter — search by division
            BEGIN
                SELECT SHELTER_ID, SHELTER_NAME, AVAILABLE_BEDS
                INTO   v_shelter_id, v_shelter_name, v_available
                FROM (
                    SELECT s.SHELTER_ID,
                           s.SHELTER_NAME,
                           sc.AVAILABLE_BEDS
                    FROM   SHELTERS         s
                    JOIN   SHELTER_CAPACITY sc ON s.SHELTER_ID = sc.SHELTER_ID
                    WHERE  s.IS_ACTIVE   = 1
                      AND  sc.AVAILABLE_BEDS > 0
                    ORDER BY sc.AVAILABLE_BEDS DESC
                ) WHERE ROWNUM = 1;

            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    p_result  := 0;
                    p_message := 'ERROR: No shelter with available capacity found nationwide.';
                    RETURN;
            END;
    END;

    -- Step 5: Assign victim to the shelter
    UPDATE VICTIMS
    SET    SHELTER_ID = v_shelter_id,
           STATUS     = 'Sheltered',
           UPDATED_AT = SYSTIMESTAMP
    WHERE  VICTIM_ID  = p_victim_id;

    -- Step 6: Insert into SHELTER_RESIDENTS (fires SHELTER_CAP_TRG)
    INSERT INTO SHELTER_RESIDENTS (SHELTER_ID, VICTIM_ID, STATUS)
    VALUES (v_shelter_id, p_victim_id, 'Active');

    -- Step 7: Update SHELTER_CAPACITY (also done by trigger, but explicit here)
    UPDATE SHELTER_CAPACITY
    SET    CURRENT_OCCUPIED = CURRENT_OCCUPIED + 1,
           AVAILABLE_BEDS   = AVAILABLE_BEDS   - 1,
           LAST_UPDATED     = SYSTIMESTAMP
    WHERE  SHELTER_ID = v_shelter_id;

    -- Step 8: Log the allocation in AUDIT_LOG
    INSERT INTO AUDIT_LOG (
        TABLE_NAME, RECORD_ID, OPERATION,
        OLD_VALUE, NEW_VALUE,
        CHANGED_BY, MODULE
    ) VALUES (
        'VICTIMS', p_victim_id, 'UPDATE',
        'STATUS=Displaced',
        'STATUS=Sheltered, SHELTER_ID=' || v_shelter_id,
        'SYSTEM', 'ALLOCATE_SHELTER'
    );

    COMMIT;

    p_result  := v_shelter_id;
    p_message := 'SUCCESS: Victim assigned to ' || v_shelter_name
                 || ' (' || v_available || ' beds available before assignment)';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_result  := -1;
        p_message := 'ERROR: ' || SQLERRM;
END ALLOCATE_SHELTER;
/

-- Test the procedure (comment out in production):
-- DECLARE
--     v_result  NUMBER;
--     v_msg     VARCHAR2(500);
-- BEGIN
--     ALLOCATE_SHELTER(1, 1, v_result, v_msg);
--     DBMS_OUTPUT.PUT_LINE('Result: ' || v_result);
--     DBMS_OUTPUT.PUT_LINE('Message: ' || v_msg);
-- END;
-- /
