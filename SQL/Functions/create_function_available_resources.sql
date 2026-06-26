-- ============================================================
-- DIEMS — Function: AVAILABLE_RESOURCES
-- Module: Emergency Resource Management
-- Oracle 24.3.1
--
-- PURPOSE:
--   Returns the total available quantity of resources under a
--   specific category name (e.g. 'Food Packets', 'Clean Water').
--
-- PARAMETERS:
--   p_category   IN VARCHAR2  - Category name to query
--
-- RETURNS: NUMBER - Total available quantity in stock
--
-- USAGE:
--   SELECT AVAILABLE_RESOURCES('Clean Water') FROM DUAL;
-- ============================================================

CREATE OR REPLACE FUNCTION AVAILABLE_RESOURCES (
    p_category IN VARCHAR2
)
RETURN NUMBER
AS
    v_total_qty NUMBER := 0;
BEGIN
    SELECT NVL(SUM(r.AVAILABLE_QUANTITY), 0)
    INTO   v_total_qty
    FROM   RESOURCES r
    JOIN   RESOURCE_CATEGORIES c ON r.CATEGORY_ID = c.CATEGORY_ID
    WHERE  UPPER(c.CATEGORY_NAME) = UPPER(p_category);

    RETURN v_total_qty;
EXCEPTION
    WHEN OTHERS THEN
        RETURN 0;
END AVAILABLE_RESOURCES;
/
