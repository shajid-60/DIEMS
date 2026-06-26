-- ============================================================
-- DIEMS — Create Table: SEVERITY_LEVELS
-- Module: Disaster Database
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE SEVERITY_LEVELS (
    LEVEL_ID    NUMBER PRIMARY KEY,
    LEVEL_NAME  VARCHAR2(20)  NOT NULL UNIQUE,
    LEVEL_CODE  NUMBER(1)     NOT NULL UNIQUE,  -- 4=Critical, 3=High, 2=Medium, 1=Low
    COLOR_CODE  VARCHAR2(10),                    -- Hex color for UI badge
    DESCRIPTION VARCHAR2(300)
);


COMMENT ON TABLE  SEVERITY_LEVELS            IS 'Reference table for disaster severity classification';
COMMENT ON COLUMN SEVERITY_LEVELS.LEVEL_CODE IS '4=Critical, 3=High, 2=Medium, 1=Low - used for AI scoring';
