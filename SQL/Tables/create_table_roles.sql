-- ============================================================
-- DIEMS — Create Table: ROLES
-- Module: User Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE ROLES (
    ROLE_ID     NUMBER PRIMARY KEY,
    ROLE_NAME   VARCHAR2(50)  NOT NULL UNIQUE,
    DESCRIPTION VARCHAR2(300),
    IS_ACTIVE   NUMBER(1)     DEFAULT 1 NOT NULL,
    CREATED_AT  TIMESTAMP     DEFAULT SYSTIMESTAMP
);

-- Comments
COMMENT ON TABLE  ROLES              IS 'System roles for user access control';
COMMENT ON COLUMN ROLES.ROLE_ID     IS 'Primary key - auto generated';
COMMENT ON COLUMN ROLES.ROLE_NAME   IS 'Unique role name e.g. ADMIN, OFFICIAL, RESPONDER, CITIZEN';
COMMENT ON COLUMN ROLES.IS_ACTIVE   IS '1 = Active, 0 = Inactive';
