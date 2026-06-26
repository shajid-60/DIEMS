-- ============================================================
-- DIEMS — Create Table: AFFECTED_AREAS
-- Module: Disaster Database
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE AFFECTED_AREAS (
    AREA_ID              NUMBER PRIMARY KEY,
    DISASTER_ID          NUMBER        NOT NULL,
    DISTRICT             VARCHAR2(100) NOT NULL,
    UPAZILA              VARCHAR2(100),
    UNION_NAME           VARCHAR2(100),
    AREA_KM2             NUMBER(10, 2),         -- Area affected in square kilometres
    POPULATION_AT_RISK   NUMBER        DEFAULT 0,
    IS_EVACUATED         NUMBER(1)     DEFAULT 0,
    EVACUATION_DATE      TIMESTAMP,
    NOTES                VARCHAR2(500),
    CREATED_AT           TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_AREAS_DISASTER
        FOREIGN KEY (DISASTER_ID)
        REFERENCES DISASTERS(DISASTER_ID) ON DELETE CASCADE,

    CONSTRAINT CHK_AREAS_EVACUATED
        CHECK (IS_EVACUATED IN (0, 1))
);

CREATE INDEX IDX_AREAS_DISASTER  ON AFFECTED_AREAS(DISASTER_ID);
CREATE INDEX IDX_AREAS_DISTRICT  ON AFFECTED_AREAS(DISTRICT);

COMMENT ON TABLE  AFFECTED_AREAS                IS 'Geographic areas affected by each disaster event';
COMMENT ON COLUMN AFFECTED_AREAS.IS_EVACUATED   IS '1 = Evacuation completed, 0 = Not yet evacuated';
COMMENT ON COLUMN AFFECTED_AREAS.AREA_KM2       IS 'Estimated area affected in square kilometres';
