-- ============================================================
-- DIEMS — Create Table: VICTIMS
-- Module: Victim Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE VICTIMS (
    VICTIM_ID         NUMBER PRIMARY KEY,
    NID               VARCHAR2(20),
    FULL_NAME         VARCHAR2(100) NOT NULL,
    AGE               NUMBER(3),
    DATE_OF_BIRTH     DATE,
    GENDER            CHAR(1)       NOT NULL,    -- M / F / O
    PHONE             VARCHAR2(20),
    EMERGENCY_CONTACT VARCHAR2(20),
    ADDRESS           VARCHAR2(300),
    DISTRICT          VARCHAR2(100),
    DISASTER_ID       NUMBER        NOT NULL,
    SHELTER_ID        NUMBER,
    MEDICAL_CONDITION VARCHAR2(20)  DEFAULT 'Stable',
    BLOOD_GROUP       VARCHAR2(5),
    STATUS            VARCHAR2(20)  DEFAULT 'Displaced' NOT NULL,
    REGISTERED_BY     NUMBER,
    REGISTERED_AT     TIMESTAMP     DEFAULT SYSTIMESTAMP,
    UPDATED_AT        TIMESTAMP     DEFAULT SYSTIMESTAMP,
    NOTES             CLOB,

    CONSTRAINT FK_VICTIMS_DISASTER
        FOREIGN KEY (DISASTER_ID)
        REFERENCES DISASTERS(DISASTER_ID),

    CONSTRAINT FK_VICTIMS_SHELTER
        FOREIGN KEY (SHELTER_ID)
        REFERENCES SHELTERS(SHELTER_ID),

    CONSTRAINT FK_VICTIMS_REGISTERED_BY
        FOREIGN KEY (REGISTERED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_VICTIMS_GENDER
        CHECK (GENDER IN ('M', 'F', 'O')),

    CONSTRAINT CHK_VICTIMS_STATUS
        CHECK (STATUS IN ('Sheltered', 'Missing', 'Medical', 'Displaced', 'Evacuated', 'Deceased')),

    CONSTRAINT CHK_VICTIMS_MED_CONDITION
        CHECK (MEDICAL_CONDITION IN ('None', 'Stable', 'Minor', 'Moderate', 'Critical', 'Deceased'))
);

CREATE INDEX IDX_VICTIMS_DISASTER   ON VICTIMS(DISASTER_ID);
CREATE INDEX IDX_VICTIMS_SHELTER    ON VICTIMS(SHELTER_ID);
CREATE INDEX IDX_VICTIMS_STATUS     ON VICTIMS(STATUS);
CREATE INDEX IDX_VICTIMS_NAME       ON VICTIMS(FULL_NAME);
CREATE INDEX IDX_VICTIMS_NID        ON VICTIMS(NID);

COMMENT ON TABLE  VICTIMS                      IS 'Disaster victims registered and tracked by the system';
COMMENT ON COLUMN VICTIMS.GENDER               IS 'M=Male, F=Female, O=Other';
COMMENT ON COLUMN VICTIMS.MEDICAL_CONDITION    IS 'Current medical status of the victim';
COMMENT ON COLUMN VICTIMS.STATUS               IS 'Current location/situation status of the victim';
COMMENT ON COLUMN VICTIMS.SHELTER_ID           IS 'Assigned shelter - auto-filled by ALLOCATE_SHELTER procedure';
