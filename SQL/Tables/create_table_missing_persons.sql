-- ============================================================
-- DIEMS — Create Table: MISSING_PERSONS
-- Module: Victim Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE MISSING_PERSONS (
    MP_ID              NUMBER PRIMARY KEY,
    FULL_NAME          VARCHAR2(100) NOT NULL,
    AGE                NUMBER(3),
    GENDER             CHAR(1),
    LAST_SEEN_DATE     TIMESTAMP,
    LAST_SEEN_LOCATION VARCHAR2(300),
    DISASTER_ID        NUMBER        NOT NULL,
    REPORTED_BY        NUMBER,
    STATUS             VARCHAR2(20)  DEFAULT 'Missing' NOT NULL,
    FOUND_DATE         TIMESTAMP,
    FOUND_LOCATION     VARCHAR2(300),
    PHOTO_PATH         VARCHAR2(500),
    DESCRIPTION        CLOB,
    PHYSICAL_DESC      VARCHAR2(300),  -- Height, clothing, identifying marks
    CONTACT_FAMILY     VARCHAR2(100),
    CONTACT_PHONE      VARCHAR2(20),
    CREATED_AT         TIMESTAMP     DEFAULT SYSTIMESTAMP,
    UPDATED_AT         TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_MP_DISASTER
        FOREIGN KEY (DISASTER_ID)
        REFERENCES DISASTERS(DISASTER_ID),

    CONSTRAINT FK_MP_REPORTED_BY
        FOREIGN KEY (REPORTED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_MP_GENDER
        CHECK (GENDER IN ('M', 'F', 'O') OR GENDER IS NULL),

    CONSTRAINT CHK_MP_STATUS
        CHECK (STATUS IN ('Missing', 'Found', 'Deceased', 'Unidentified'))
);

CREATE INDEX IDX_MP_DISASTER  ON MISSING_PERSONS(DISASTER_ID);
CREATE INDEX IDX_MP_STATUS    ON MISSING_PERSONS(STATUS);
CREATE INDEX IDX_MP_NAME      ON MISSING_PERSONS(FULL_NAME);

COMMENT ON TABLE  MISSING_PERSONS               IS 'Missing persons reported during disaster events';
COMMENT ON COLUMN MISSING_PERSONS.PHYSICAL_DESC IS 'Physical description to aid identification';
COMMENT ON COLUMN MISSING_PERSONS.STATUS        IS 'Missing=actively searching, Found=located, Deceased=confirmed dead';
