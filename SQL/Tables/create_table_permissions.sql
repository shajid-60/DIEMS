-- ============================================================
-- DIEMS — Create Table: PERMISSIONS
-- Module: User Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE PERMISSIONS (
    PERMISSION_ID   NUMBER PRIMARY KEY,
    PERMISSION_NAME VARCHAR2(100) NOT NULL UNIQUE,
    MODULE          VARCHAR2(50)  NOT NULL,
    ACTION          VARCHAR2(30)  NOT NULL,   -- VIEW, CREATE, UPDATE, DELETE
    DESCRIPTION     VARCHAR2(300),
    CREATED_AT      TIMESTAMP DEFAULT SYSTIMESTAMP
);

COMMENT ON TABLE  PERMISSIONS                 IS 'Fine-grained permissions per module and action';
COMMENT ON COLUMN PERMISSIONS.MODULE          IS 'e.g. DISASTER, VICTIM, RESOURCE, SHELTER';
COMMENT ON COLUMN PERMISSIONS.ACTION          IS 'e.g. VIEW, CREATE, UPDATE, DELETE, APPROVE';
