-- ============================================================
-- DIEMS — Create View: CRITICAL_RESOURCES_VW
-- Module: Emergency Resource Management
-- Oracle 24.3.1
-- ============================================================

CREATE OR REPLACE VIEW CRITICAL_RESOURCES_VW AS
SELECT
    r.RESOURCE_ID,
    r.RESOURCE_NAME,
    rc.CATEGORY_NAME,
    rc.UNIT,
    rc.ICON,
    rc.CRITICAL_THRESHOLD,
    r.TOTAL_QUANTITY,
    r.AVAILABLE_QUANTITY,
    r.RESERVED_QUANTITY,
    -- Stock percentage
    ROUND((r.AVAILABLE_QUANTITY / NULLIF(r.TOTAL_QUANTITY, 0)) * 100, 1)
                                AS STOCK_PCT,
    -- Quantity below threshold
    r.TOTAL_QUANTITY - r.AVAILABLE_QUANTITY
                                AS QUANTITY_USED,
    -- Alert level
    CASE
        WHEN ROUND((r.AVAILABLE_QUANTITY / NULLIF(r.TOTAL_QUANTITY, 0)) * 100, 1) <= 15
            THEN 'CRITICAL'
        WHEN ROUND((r.AVAILABLE_QUANTITY / NULLIF(r.TOTAL_QUANTITY, 0)) * 100, 1) <= rc.CRITICAL_THRESHOLD
            THEN 'LOW'
        ELSE 'NORMAL'
    END                         AS ALERT_LEVEL,
    r.STORAGE_LOCATION,
    r.SUPPLIER_NAME,
    r.SUPPLIER_CONTACT,
    r.LAST_UPDATED,
    u.FULL_NAME                 AS LAST_UPDATED_BY
FROM
    RESOURCES           r
    JOIN RESOURCE_CATEGORIES rc ON r.CATEGORY_ID = rc.CATEGORY_ID
    LEFT JOIN USERS          u  ON r.UPDATED_BY  = u.USER_ID
WHERE
    -- Show resources at or below their critical threshold
    ROUND((r.AVAILABLE_QUANTITY / NULLIF(r.TOTAL_QUANTITY, 0)) * 100, 1)
        <= rc.CRITICAL_THRESHOLD
ORDER BY
    STOCK_PCT ASC;   -- Most critical first

COMMENT ON TABLE CRITICAL_RESOURCES_VW IS
    'View: Resources below their critical stock threshold. Triggers ALERT_THRESHOLD_TRG. Used by Dashboard warning panel and Resource Management page.';
