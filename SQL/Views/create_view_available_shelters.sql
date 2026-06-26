-- ============================================================
-- DIEMS — Create View: AVAILABLE_SHELTERS_VW
-- Module: Shelter Management
-- Oracle 24.3.1
-- ============================================================

CREATE OR REPLACE VIEW AVAILABLE_SHELTERS_VW AS
SELECT
    s.SHELTER_ID,
    s.SHELTER_NAME,
    s.SHELTER_TYPE,
    s.LOCATION,
    s.DISTRICT,
    s.UPAZILA,
    s.LATITUDE,
    s.LONGITUDE,
    s.CONTACT_PERSON,
    s.CONTACT_PHONE,
    s.HAS_MEDICAL,
    s.HAS_GENERATOR,
    s.HAS_WIFI,
    s.FACILITIES,
    sc.MAX_CAPACITY,
    sc.CURRENT_OCCUPIED,
    sc.AVAILABLE_BEDS,
    sc.RESERVED_SPOTS,
    sc.HAS_OVERFLOW,
    -- Occupancy percentage
    ROUND((sc.CURRENT_OCCUPIED / NULLIF(sc.MAX_CAPACITY, 0)) * 100, 1)
                                            AS OCCUPANCY_PCT,
    -- Status label
    CASE
        WHEN ROUND((sc.CURRENT_OCCUPIED / NULLIF(sc.MAX_CAPACITY, 0)) * 100, 1) >= 95
            THEN 'FULL'
        WHEN ROUND((sc.CURRENT_OCCUPIED / NULLIF(sc.MAX_CAPACITY, 0)) * 100, 1) >= 80
            THEN 'NEAR CAPACITY'
        ELSE 'AVAILABLE'
    END                                     AS CAPACITY_STATUS,
    -- Remaining capacity
    (sc.MAX_CAPACITY - sc.CURRENT_OCCUPIED) AS REMAINING_CAPACITY
FROM
    SHELTERS         s
    JOIN SHELTER_CAPACITY sc ON s.SHELTER_ID = sc.SHELTER_ID
WHERE
    s.IS_ACTIVE        = 1
    AND sc.AVAILABLE_BEDS > 0
ORDER BY
    sc.AVAILABLE_BEDS DESC;

COMMENT ON TABLE AVAILABLE_SHELTERS_VW IS
    'View: All shelters with available beds, occupancy %, and status labels. Used for victim assignment and dashboard map.';
