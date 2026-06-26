-- ============================================================
-- DIEMS — Create Table: VOLUNTEERS
-- Module: Volunteer Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE VOLUNTEERS (
    VOLUNTEER_ID     NUMBER PRIMARY KEY,
    USER_ID          NUMBER,                   -- NULL if not a system user
    FULL_NAME        VARCHAR2(100) NOT NULL,
    PHONE            VARCHAR2(20)  NOT NULL,
    EMAIL            VARCHAR2(100),
    NID              VARCHAR2(20),
    DATE_OF_BIRTH    DATE,
    GENDER           CHAR(1),
    DISTRICT         VARCHAR2(100),
    ADDRESS          VARCHAR2(300),
    ORGANIZATION     VARCHAR2(150),            -- NGO / Company / Independent
    AVAILABILITY     VARCHAR2(20)  DEFAULT 'Available' NOT NULL,
    EXPERIENCE_YEARS NUMBER(2)     DEFAULT 0,
    LANGUAGES        VARCHAR2(200),            -- Bangla, English, etc.
    JOINED_DATE      DATE          DEFAULT SYSDATE,
    TOTAL_MISSIONS   NUMBER        DEFAULT 0,
    RATING           NUMBER(3,1),              -- Average rating 1.0 - 5.0
    IS_VERIFIED      NUMBER(1)     DEFAULT 0,  -- 1 = Background-checked
    IS_ACTIVE        NUMBER(1)     DEFAULT 1,
    EMERGENCY_CONTACT VARCHAR2(100),
    EMERGENCY_PHONE   VARCHAR2(20),
    NOTES            VARCHAR2(500),
    CREATED_AT       TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_VOL_USER
        FOREIGN KEY (USER_ID)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_VOL_GENDER
        CHECK (GENDER IN ('M', 'F', 'O') OR GENDER IS NULL),

    CONSTRAINT CHK_VOL_AVAILABILITY
        CHECK (AVAILABILITY IN ('Available', 'Assigned', 'Unavailable', 'On Leave')),

    CONSTRAINT CHK_VOL_RATING
        CHECK (RATING IS NULL OR (RATING >= 1.0 AND RATING <= 5.0)),

    CONSTRAINT CHK_VOL_VERIFIED
        CHECK (IS_VERIFIED IN (0, 1))
);

CREATE INDEX IDX_VOL_DISTRICT     ON VOLUNTEERS(DISTRICT);
CREATE INDEX IDX_VOL_AVAILABILITY ON VOLUNTEERS(AVAILABILITY);
CREATE INDEX IDX_VOL_USER         ON VOLUNTEERS(USER_ID);

COMMENT ON TABLE  VOLUNTEERS              IS 'Registered volunteers for disaster response operations';
COMMENT ON COLUMN VOLUNTEERS.IS_VERIFIED  IS '1 = Identity and background verified by admin';
COMMENT ON COLUMN VOLUNTEERS.TOTAL_MISSIONS IS 'Counter - incremented after each completed assignment';
