-- ============================================================
-- DIEMS — Create Table: AMBULANCES
-- Module: Hospital & Medical Support
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE AMBULANCES (
    AMBULANCE_ID       NUMBER PRIMARY KEY,
    VEHICLE_NO         VARCHAR2(30)  NOT NULL UNIQUE,
    HOSPITAL_ID        NUMBER        NOT NULL,
    AMBULANCE_TYPE     VARCHAR2(50)  DEFAULT 'Basic',  -- Basic, Advanced, ICU Mobile
    STATUS             VARCHAR2(20)  DEFAULT 'Available' NOT NULL,
    DRIVER_NAME        VARCHAR2(100),
    DRIVER_PHONE       VARCHAR2(20),
    PARAMEDIC_NAME     VARCHAR2(100),
    PARAMEDIC_PHONE    VARCHAR2(20),
    CURRENT_LOCATION   VARCHAR2(300),
    ASSIGNED_DISASTER  NUMBER,
    LATITUDE           NUMBER(10, 6),
    LONGITUDE          NUMBER(10, 6),
    LAST_UPDATED       TIMESTAMP     DEFAULT SYSTIMESTAMP,
    NOTES              VARCHAR2(300),

    CONSTRAINT FK_AMB_HOSPITAL
        FOREIGN KEY (HOSPITAL_ID)
        REFERENCES HOSPITALS(HOSPITAL_ID),

    CONSTRAINT FK_AMB_DISASTER
        FOREIGN KEY (ASSIGNED_DISASTER)
        REFERENCES DISASTERS(DISASTER_ID),

    CONSTRAINT CHK_AMB_STATUS
        CHECK (STATUS IN ('Available', 'Deployed', 'En Route', 'Maintenance', 'Out of Service')),

    CONSTRAINT CHK_AMB_TYPE
        CHECK (AMBULANCE_TYPE IN ('Basic', 'Advanced', 'ICU Mobile', 'Motorcycle', 'Boat'))
);

CREATE INDEX IDX_AMB_HOSPITAL  ON AMBULANCES(HOSPITAL_ID);
CREATE INDEX IDX_AMB_STATUS    ON AMBULANCES(STATUS);
CREATE INDEX IDX_AMB_DISASTER  ON AMBULANCES(ASSIGNED_DISASTER);

COMMENT ON TABLE  AMBULANCES                    IS 'Ambulance fleet registered across all hospitals';
COMMENT ON COLUMN AMBULANCES.AMBULANCE_TYPE     IS 'Basic=first aid, Advanced=ALS, ICU Mobile=critical care en route';
COMMENT ON COLUMN AMBULANCES.ASSIGNED_DISASTER  IS 'Currently deployed to which disaster zone';
