-- ============================================================
-- DIEMS — Create Table: USERS
-- Module: User Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE USERS (
    USER_ID       NUMBER PRIMARY KEY,
    USERNAME      VARCHAR2(50)  NOT NULL UNIQUE,
    PASSWORD_HASH VARCHAR2(256) NOT NULL,
    EMAIL         VARCHAR2(100) UNIQUE,
    FULL_NAME     VARCHAR2(100) NOT NULL,
    PHONE         VARCHAR2(20),
    NID           VARCHAR2(20),
    ROLE_ID       NUMBER        NOT NULL,
    DISTRICT      VARCHAR2(100),
    ADDRESS       VARCHAR2(300),
    PROFILE_PIC   VARCHAR2(500),
    IS_ACTIVE     NUMBER(1)     DEFAULT 1  NOT NULL,
    CREATED_AT    TIMESTAMP     DEFAULT SYSTIMESTAMP,
    LAST_LOGIN    TIMESTAMP,
    CONSTRAINT FK_USERS_ROLE FOREIGN KEY (ROLE_ID)
        REFERENCES ROLES(ROLE_ID)
);

-- Index on commonly queried columns
-- Note: USERNAME and EMAIL already have implicit unique indexes due to UNIQUE constraints
CREATE INDEX IDX_USERS_ROLE     ON USERS(ROLE_ID);

COMMENT ON TABLE  USERS               IS 'System users: admins, officials, responders, citizens';
COMMENT ON COLUMN USERS.PASSWORD_HASH IS 'BCrypt or SHA-256 hashed password - never store plaintext';
COMMENT ON COLUMN USERS.NID           IS 'National Identity Card number';
COMMENT ON COLUMN USERS.IS_ACTIVE     IS '1 = Active, 0 = Deactivated/Banned';
