-- ============================================================
-- DIEMS — Create Table: FAMILY_MEMBERS
-- Module: Victim Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE FAMILY_MEMBERS (
    FM_ID             NUMBER PRIMARY KEY,
    VICTIM_ID         NUMBER        NOT NULL,
    FULL_NAME         VARCHAR2(100) NOT NULL,
    RELATION          VARCHAR2(50)  NOT NULL,   -- Father, Mother, Spouse, Son, Daughter etc.
    AGE               NUMBER(3),
    GENDER            CHAR(1),
    PHONE             VARCHAR2(20),
    IS_SEPARATED      NUMBER(1)     DEFAULT 0,  -- 1 = separated from victim during disaster
    LAST_KNOWN_LOC    VARCHAR2(300),
    STATUS            VARCHAR2(20)  DEFAULT 'Unknown',
    NOTES             VARCHAR2(500),
    CREATED_AT        TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_FM_VICTIM
        FOREIGN KEY (VICTIM_ID)
        REFERENCES VICTIMS(VICTIM_ID) ON DELETE CASCADE,

    CONSTRAINT CHK_FM_GENDER
        CHECK (GENDER IN ('M', 'F', 'O') OR GENDER IS NULL),

    CONSTRAINT CHK_FM_SEPARATED
        CHECK (IS_SEPARATED IN (0, 1)),

    CONSTRAINT CHK_FM_STATUS
        CHECK (STATUS IN ('Safe', 'Missing', 'Deceased', 'Unknown', 'Injured'))
);

CREATE INDEX IDX_FM_VICTIM     ON FAMILY_MEMBERS(VICTIM_ID);
CREATE INDEX IDX_FM_SEPARATED  ON FAMILY_MEMBERS(IS_SEPARATED);

COMMENT ON TABLE  FAMILY_MEMBERS              IS 'Family members linked to registered victims';
COMMENT ON COLUMN FAMILY_MEMBERS.IS_SEPARATED IS '1 = separated from main victim during disaster - needs tracing';
COMMENT ON COLUMN FAMILY_MEMBERS.STATUS       IS 'Current status of this family member';
