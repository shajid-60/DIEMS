-- ============================================================
-- DIEMS — Create View: ACTIVE_DISASTERS_VW
-- Module: Disaster Database
-- Oracle 24.3.1
-- ============================================================

CREATE OR REPLACE VIEW ACTIVE_DISASTERS_VW AS
SELECT
    d.DISASTER_ID,
    d.DISASTER_NAME,
    dt.TYPE_NAME,
    dt.ICON                          AS TYPE_ICON,
    sl.LEVEL_NAME                    AS SEVERITY,
    sl.LEVEL_CODE                    AS SEVERITY_CODE,
    sl.COLOR_CODE                    AS SEVERITY_COLOR,
    d.DISTRICT,
    d.DIVISION,
    d.LATITUDE,
    d.LONGITUDE,
    d.START_DATE,
    d.STATUS,
    d.AFFECTED_POPULATION,
    d.CASUALTIES,
    d.INJURED,
    d.DISPLACED,
    d.ESTIMATED_DAMAGE,
    d.RESPONSE_TEAMS,
    d.DESCRIPTION,
    TRUNC(SYSDATE - CAST(d.START_DATE AS DATE))  AS DAYS_ACTIVE,
    u.FULL_NAME                      AS REPORTED_BY_NAME
FROM
    DISASTERS        d
    JOIN DISASTER_TYPES   dt ON d.TYPE_ID           = dt.TYPE_ID
    JOIN SEVERITY_LEVELS  sl ON d.SEVERITY_LEVEL_ID = sl.LEVEL_ID
    LEFT JOIN USERS       u  ON d.REPORTED_BY       = u.USER_ID
WHERE
    d.STATUS IN ('ACTIVE', 'MONITORING')
ORDER BY
    sl.LEVEL_CODE DESC,   -- Critical first
    d.START_DATE   DESC;

-- Grant read access
-- GRANT SELECT ON ACTIVE_DISASTERS_VW TO PUBLIC;

COMMENT ON TABLE ACTIVE_DISASTERS_VW IS 
    'View: All currently ACTIVE or MONITORING disaster events with full type and severity details. Used by Dashboard, Map, and Alert systems.';
