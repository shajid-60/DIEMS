-- ============================================================
-- DIEMS — Create Table: SHELTERS
-- Module: Shelter Management
-- Oracle 24.3.1
-- NOTE: Created BEFORE VICTIMS because VICTIMS references SHELTERS
-- ============================================================

CREATE TABLE SHELTERS (
    SHELTER_ID      NUMBER PRIMARY KEY,
    SHELTER_NAME    VARCHAR2(200) NOT NULL,
    SHELTER_TYPE    VARCHAR2(50)  DEFAULT 'General',  -- General, Medical, Women, Children
    LOCATION        VARCHAR2(300) NOT NULL,
    DISTRICT        VARCHAR2(100) NOT NULL,
    UPAZILA         VARCHAR2(100),
    LATITUDE        NUMBER(10, 6),
    LONGITUDE       NUMBER(10, 6),
    CONTACT_PERSON  VARCHAR2(100),
    CONTACT_PHONE   VARCHAR2(20),
    FACILITIES      CLOB,                             -- JSON or comma-separated list
    HAS_MEDICAL     NUMBER(1)     DEFAULT 0,
    HAS_GENERATOR   NUMBER(1)     DEFAULT 0,
    HAS_WIFI        NUMBER(1)     DEFAULT 0,
    IS_ACTIVE       NUMBER(1)     DEFAULT 1  NOT NULL,
    OPENED_DATE     DATE          DEFAULT SYSDATE,
    CLOSED_DATE     DATE,
    CREATED_BY      NUMBER,
    CREATED_AT      TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_SHELTERS_CREATOR
        FOREIGN KEY (CREATED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_SHELTERS_ACTIVE
        CHECK (IS_ACTIVE IN (0, 1)),

    CONSTRAINT CHK_SHELTERS_TYPE
        CHECK (SHELTER_TYPE IN ('General','Medical','Women','Children','Elderly','Mixed'))
);

CREATE INDEX IDX_SHELTERS_DISTRICT ON SHELTERS(DISTRICT);
CREATE INDEX IDX_SHELTERS_ACTIVE   ON SHELTERS(IS_ACTIVE);

COMMENT ON TABLE  SHELTERS              IS 'Emergency shelters registered in the system';
COMMENT ON COLUMN SHELTERS.FACILITIES   IS 'Comma-separated: Food,Water,Medical,Toilets,Generator,WiFi';
COMMENT ON COLUMN SHELTERS.SHELTER_TYPE IS 'Classification of shelter by target demographic';
