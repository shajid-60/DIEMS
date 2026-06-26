-- ============================================================
-- DIEMS — Create Table: AUDIT_LOG
-- Module: System Audit / Security
-- Oracle 24.3.1
-- NOTE: Populated by AUDIT_CHANGES_TRG (AFTER UPDATE on DISASTERS)
--       and can be written to by any trigger across the system
-- ============================================================

CREATE TABLE AUDIT_LOG (
    LOG_ID       NUMBER PRIMARY KEY,
    TABLE_NAME   VARCHAR2(100) NOT NULL,
    RECORD_ID    NUMBER,                     -- PK of the changed record
    OPERATION    VARCHAR2(10)  NOT NULL,     -- INSERT / UPDATE / DELETE
    COLUMN_NAME  VARCHAR2(100),              -- Which column changed (for UPDATE)
    OLD_VALUE    CLOB,                       -- Previous value (JSON or plain text)
    NEW_VALUE    CLOB,                       -- New value (JSON or plain text)
    CHANGED_BY   VARCHAR2(100),              -- USERNAME or 'SYSTEM'
    CHANGED_AT   TIMESTAMP     DEFAULT SYSTIMESTAMP NOT NULL,
    IP_ADDRESS   VARCHAR2(50),
    SESSION_ID   VARCHAR2(100),
    MODULE       VARCHAR2(50),               -- Which application module triggered it
    NOTES        VARCHAR2(500),

    CONSTRAINT CHK_AUDIT_OPERATION
        CHECK (OPERATION IN ('INSERT', 'UPDATE', 'DELETE', 'LOGIN', 'LOGOUT', 'FAILED_LOGIN'))
);

-- Indexes for audit queries
CREATE INDEX IDX_AUDIT_TABLE    ON AUDIT_LOG(TABLE_NAME);
CREATE INDEX IDX_AUDIT_RECORD   ON AUDIT_LOG(RECORD_ID);
CREATE INDEX IDX_AUDIT_USER     ON AUDIT_LOG(CHANGED_BY);
CREATE INDEX IDX_AUDIT_DATE     ON AUDIT_LOG(CHANGED_AT);
CREATE INDEX IDX_AUDIT_OP       ON AUDIT_LOG(OPERATION);

COMMENT ON TABLE  AUDIT_LOG              IS 'Full audit trail for all data changes across the system';
COMMENT ON COLUMN AUDIT_LOG.OLD_VALUE    IS 'Serialized old row data before the change';
COMMENT ON COLUMN AUDIT_LOG.NEW_VALUE    IS 'Serialized new row data after the change';
COMMENT ON COLUMN AUDIT_LOG.CHANGED_BY  IS 'Username of person who made the change or SYSTEM for trigger-based';
COMMENT ON COLUMN AUDIT_LOG.OPERATION   IS 'Type of DML operation or authentication event';
