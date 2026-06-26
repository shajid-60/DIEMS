-- ============================================================
-- DIEMS — Create Table: HOSPITALS
-- Module: Hospital & Medical Support
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE HOSPITALS (
    HOSPITAL_ID      NUMBER PRIMARY KEY,
    HOSPITAL_NAME    VARCHAR2(200) NOT NULL,
    HOSPITAL_TYPE    VARCHAR2(50)  DEFAULT 'Government',
    DISTRICT         VARCHAR2(100) NOT NULL,
    UPAZILA          VARCHAR2(100),
    ADDRESS          VARCHAR2(300),
    CONTACT_NUMBER   VARCHAR2(20),
    EMAIL            VARCHAR2(100),
    TOTAL_BEDS       NUMBER        DEFAULT 0,
    AVAILABLE_BEDS   NUMBER        DEFAULT 0,
    ICU_BEDS         NUMBER        DEFAULT 0,
    ICU_AVAILABLE    NUMBER        DEFAULT 0,
    BLOOD_O_POS      NUMBER        DEFAULT 0,
    BLOOD_O_NEG      NUMBER        DEFAULT 0,
    BLOOD_A_POS      NUMBER        DEFAULT 0,
    BLOOD_A_NEG      NUMBER        DEFAULT 0,
    BLOOD_B_POS      NUMBER        DEFAULT 0,
    BLOOD_B_NEG      NUMBER        DEFAULT 0,
    BLOOD_AB_POS     NUMBER        DEFAULT 0,
    BLOOD_AB_NEG     NUMBER        DEFAULT 0,
    HAS_EMERGENCY    NUMBER(1)     DEFAULT 1,
    HAS_AMBULANCE    NUMBER(1)     DEFAULT 0,
    IS_ACTIVE        NUMBER(1)     DEFAULT 1  NOT NULL,
    LATITUDE         NUMBER(10, 6),
    LONGITUDE        NUMBER(10, 6),
    CREATED_AT       TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT CHK_HOSP_TYPE
        CHECK (HOSPITAL_TYPE IN ('Government','Private','NGO','Military','Field Hospital')),

    CONSTRAINT CHK_HOSP_BEDS
        CHECK (AVAILABLE_BEDS <= TOTAL_BEDS AND AVAILABLE_BEDS >= 0)
);

CREATE INDEX IDX_HOSP_DISTRICT ON HOSPITALS(DISTRICT);
CREATE INDEX IDX_HOSP_ACTIVE   ON HOSPITALS(IS_ACTIVE);

COMMENT ON TABLE  HOSPITALS              IS 'Hospitals registered in the emergency medical response network';
COMMENT ON COLUMN HOSPITALS.ICU_BEDS     IS 'Total ICU beds in the hospital';
COMMENT ON COLUMN HOSPITALS.ICU_AVAILABLE IS 'Currently available ICU beds';
COMMENT ON COLUMN HOSPITALS.HAS_EMERGENCY IS '1 = has 24/7 emergency department';
