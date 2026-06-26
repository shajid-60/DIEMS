-- ============================================================
-- DIEMS — Create Table: DOCTORS
-- Module: Hospital & Medical Support
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE DOCTORS (
    DOCTOR_ID            NUMBER PRIMARY KEY,
    HOSPITAL_ID          NUMBER        NOT NULL,
    FULL_NAME            VARCHAR2(100) NOT NULL,
    BMDC_NUMBER          VARCHAR2(20),          -- Bangladesh Medical & Dental Council reg. no.
    SPECIALIZATION       VARCHAR2(100) NOT NULL,
    PHONE                VARCHAR2(20),
    EMAIL                VARCHAR2(100),
    EXPERIENCE_YEARS     NUMBER(3),
    IS_AVAILABLE         NUMBER(1)     DEFAULT 1,
    ASSIGNED_DISASTER_ID NUMBER,
    DEPLOYMENT_LOCATION  VARCHAR2(200),
    AVAILABILITY_STATUS  VARCHAR2(30)  DEFAULT 'Available',
    CREATED_AT           TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_DOCTORS_HOSPITAL
        FOREIGN KEY (HOSPITAL_ID)
        REFERENCES HOSPITALS(HOSPITAL_ID),

    CONSTRAINT FK_DOCTORS_DISASTER
        FOREIGN KEY (ASSIGNED_DISASTER_ID)
        REFERENCES DISASTERS(DISASTER_ID),

    CONSTRAINT CHK_DOCTORS_AVAILABLE
        CHECK (IS_AVAILABLE IN (0, 1)),

    CONSTRAINT CHK_DOCTORS_STATUS
        CHECK (AVAILABILITY_STATUS IN ('Available','On Duty','Deployed','On Leave','Off Duty'))
);

CREATE INDEX IDX_DOCTORS_HOSPITAL  ON DOCTORS(HOSPITAL_ID);
CREATE INDEX IDX_DOCTORS_AVAILABLE ON DOCTORS(IS_AVAILABLE);
CREATE INDEX IDX_DOCTORS_DISASTER  ON DOCTORS(ASSIGNED_DISASTER_ID);

COMMENT ON TABLE  DOCTORS                       IS 'Doctors registered in the emergency medical response network';
COMMENT ON COLUMN DOCTORS.BMDC_NUMBER           IS 'Bangladesh Medical and Dental Council registration number';
COMMENT ON COLUMN DOCTORS.ASSIGNED_DISASTER_ID  IS 'Current disaster deployment - NULL if at home hospital';
