-- ============================================================
-- DIEMS — Create Table: MEDICAL_REQUESTS
-- Module: Hospital & Medical Support
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE MEDICAL_REQUESTS (
    REQUEST_ID       NUMBER PRIMARY KEY,
    HOSPITAL_ID      NUMBER        NOT NULL,
    DISASTER_ID      NUMBER,
    REQUEST_TYPE     VARCHAR2(50)  NOT NULL,   -- Blood, Equipment, Doctors, Medicine, Ambulance
    REQUEST_DETAILS  CLOB          NOT NULL,
    QUANTITY_NEEDED  NUMBER,
    BLOOD_GROUP      VARCHAR2(5),              -- For blood requests: O+, AB- etc.
    PRIORITY         VARCHAR2(20)  DEFAULT 'Normal' NOT NULL,
    STATUS           VARCHAR2(20)  DEFAULT 'Pending' NOT NULL,
    REQUESTED_BY     NUMBER        NOT NULL,
    REQUESTED_AT     TIMESTAMP     DEFAULT SYSTIMESTAMP,
    ASSIGNED_TO      NUMBER,                   -- User responsible for fulfillment
    FULFILLED_AT     TIMESTAMP,
    FULFILLED_BY     NUMBER,
    RESPONSE_NOTES   VARCHAR2(500),

    CONSTRAINT FK_MEDREQ_HOSPITAL
        FOREIGN KEY (HOSPITAL_ID)
        REFERENCES HOSPITALS(HOSPITAL_ID),

    CONSTRAINT FK_MEDREQ_DISASTER
        FOREIGN KEY (DISASTER_ID)
        REFERENCES DISASTERS(DISASTER_ID),

    CONSTRAINT FK_MEDREQ_BY
        FOREIGN KEY (REQUESTED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT FK_MEDREQ_FULFILLED
        FOREIGN KEY (FULFILLED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_MEDREQ_TYPE
        CHECK (REQUEST_TYPE IN ('Blood','Equipment','Doctors','Medicine','Ambulance','ICU Bed','Other')),

    CONSTRAINT CHK_MEDREQ_PRIORITY
        CHECK (PRIORITY IN ('Emergency', 'Critical', 'High', 'Normal', 'Low')),

    CONSTRAINT CHK_MEDREQ_STATUS
        CHECK (STATUS IN ('Pending', 'Approved', 'In Progress', 'Fulfilled', 'Cancelled', 'Rejected'))
);

CREATE INDEX IDX_MEDREQ_HOSPITAL  ON MEDICAL_REQUESTS(HOSPITAL_ID);
CREATE INDEX IDX_MEDREQ_STATUS    ON MEDICAL_REQUESTS(STATUS);
CREATE INDEX IDX_MEDREQ_PRIORITY  ON MEDICAL_REQUESTS(PRIORITY);
CREATE INDEX IDX_MEDREQ_DISASTER  ON MEDICAL_REQUESTS(DISASTER_ID);

COMMENT ON TABLE  MEDICAL_REQUESTS             IS 'Emergency medical supply and resource requests from hospitals';
COMMENT ON COLUMN MEDICAL_REQUESTS.BLOOD_GROUP IS 'Required blood group for blood supply requests e.g. O+, AB-';
