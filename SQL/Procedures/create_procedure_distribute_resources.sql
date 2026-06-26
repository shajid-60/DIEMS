-- ============================================================
-- DIEMS — Stored Procedure: DISTRIBUTE_RESOURCES
-- Module: Emergency Resource Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Records a resource distribution event and automatically
--   deducts the quantity from the RESOURCES inventory.
--   Validates sufficient stock before distribution.
--   The INVENTORY_UPDATE_TRG also fires on INSERT to
--   RESOURCE_DISTRIBUTION for redundancy.
--
-- PARAMETERS:
--   p_resource_id   IN  NUMBER  - Resource being distributed
--   p_quantity      IN  NUMBER  - Quantity to distribute
--   p_shelter_id    IN  NUMBER  - Destination shelter (or NULL)
--   p_disaster_id   IN  NUMBER  - Related disaster
--   p_distributed_by IN NUMBER  - User performing distribution
--   p_priority      IN  VARCHAR2 - Immediate/High/Normal/Low
--   p_notes         IN  VARCHAR2 - Optional notes
--   p_dist_id       OUT NUMBER  - Created DIST_ID
--   p_message       OUT VARCHAR2 - Status message
-- ============================================================

CREATE OR REPLACE PROCEDURE DISTRIBUTE_RESOURCES (
    p_resource_id    IN  NUMBER,
    p_quantity       IN  NUMBER,
    p_shelter_id     IN  NUMBER,
    p_disaster_id    IN  NUMBER,
    p_distributed_by IN  NUMBER,
    p_priority       IN  VARCHAR2 DEFAULT 'Normal',
    p_notes          IN  VARCHAR2 DEFAULT NULL,
    p_dist_id        OUT NUMBER,
    p_message        OUT VARCHAR2
)
AS
    v_available      NUMBER;
    v_resource_name  VARCHAR2(150);
    v_category_name  VARCHAR2(100);
    v_threshold      NUMBER;
    v_new_pct        NUMBER;
    v_shelter_name   VARCHAR2(200);

BEGIN
    -- Step 1: Validate input
    IF p_quantity <= 0 THEN
        p_dist_id := -1;
        p_message := 'ERROR: Quantity must be greater than zero.';
        RETURN;
    END IF;

    -- Step 2: Check current available stock
    SELECT r.AVAILABLE_QUANTITY,
           r.RESOURCE_NAME,
           rc.CATEGORY_NAME,
           rc.CRITICAL_THRESHOLD
    INTO   v_available,
           v_resource_name,
           v_category_name,
           v_threshold
    FROM   RESOURCES r
    JOIN   RESOURCE_CATEGORIES rc ON r.CATEGORY_ID = rc.CATEGORY_ID
    WHERE  r.RESOURCE_ID = p_resource_id
    FOR UPDATE;   -- Lock the row during transaction

    -- Step 3: Verify sufficient stock
    IF v_available < p_quantity THEN
        p_dist_id := -2;
        p_message := 'ERROR: Insufficient stock. Available: '
                     || v_available || ', Requested: ' || p_quantity;
        RETURN;
    END IF;

    -- Step 4: Get shelter name for message
    IF p_shelter_id IS NOT NULL THEN
        SELECT SHELTER_NAME
        INTO   v_shelter_name
        FROM   SHELTERS
        WHERE  SHELTER_ID = p_shelter_id;
    ELSE
        v_shelter_name := 'Field Operation';
    END IF;

    -- Step 5: Insert distribution record
    -- NOTE: INVENTORY_UPDATE_TRG fires here as well (AFTER INSERT)
    INSERT INTO RESOURCE_DISTRIBUTION (
        RESOURCE_ID, QUANTITY, SHELTER_ID, DISASTER_ID,
        PRIORITY, DISTRIBUTED_BY, STATUS, NOTES
    ) VALUES (
        p_resource_id, p_quantity, p_shelter_id, p_disaster_id,
        p_priority, p_distributed_by, 'In Transit', p_notes
    )
    RETURNING DIST_ID INTO p_dist_id;

    -- Step 6: Deduct from RESOURCES inventory (now handled automatically by INVENTORY_UPDATE_TRG)
    -- UPDATE RESOURCES
    -- SET    AVAILABLE_QUANTITY = AVAILABLE_QUANTITY - p_quantity,
    --        LAST_UPDATED       = SYSTIMESTAMP,
    --        UPDATED_BY         = p_distributed_by
    -- WHERE  RESOURCE_ID = p_resource_id;

    -- Step 7: Calculate new stock percentage for alert check
    SELECT ROUND((AVAILABLE_QUANTITY / NULLIF(TOTAL_QUANTITY, 0)) * 100, 1)
    INTO   v_new_pct
    FROM   RESOURCES
    WHERE  RESOURCE_ID = p_resource_id;

    -- Step 8: Log to AUDIT_LOG
    INSERT INTO AUDIT_LOG (
        TABLE_NAME, RECORD_ID, OPERATION,
        OLD_VALUE, NEW_VALUE,
        CHANGED_BY, MODULE
    ) VALUES (
        'RESOURCES', p_resource_id, 'UPDATE',
        'AVAILABLE_QUANTITY=' || v_available,
        'AVAILABLE_QUANTITY=' || (v_available - p_quantity)
            || ' (Stock: ' || v_new_pct || '%)',
        (SELECT USERNAME FROM USERS WHERE USER_ID = p_distributed_by),
        'DISTRIBUTE_RESOURCES'
    );

    COMMIT;

    -- Step 9: Build success message with low-stock warning
    p_message := 'SUCCESS: ' || p_quantity || ' ' || v_category_name
                 || ' distributed to ' || v_shelter_name
                 || '. New stock: ' || (v_available - p_quantity)
                 || ' (' || v_new_pct || '%)';

    -- Append warning if below threshold
    IF v_new_pct <= v_threshold THEN
        p_message := p_message || ' — WARNING: Stock below '
                     || v_threshold || '% threshold!';
    END IF;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        ROLLBACK;
        p_dist_id := -3;
        p_message := 'ERROR: Resource ID ' || p_resource_id || ' not found.';
    WHEN OTHERS THEN
        ROLLBACK;
        p_dist_id := -99;
        p_message := 'ERROR: ' || SQLERRM;
END DISTRIBUTE_RESOURCES;
/
