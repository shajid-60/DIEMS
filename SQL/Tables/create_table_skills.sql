-- ============================================================
-- DIEMS — Create Table: SKILLS
-- Module: Volunteer Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE SKILLS (
    SKILL_ID     NUMBER PRIMARY KEY,
    SKILL_NAME   VARCHAR2(100) NOT NULL UNIQUE,
    CATEGORY     VARCHAR2(50)  NOT NULL,
    DESCRIPTION  VARCHAR2(300),
    IS_ACTIVE    NUMBER(1)     DEFAULT 1
);


COMMENT ON TABLE SKILLS IS 'Skill reference list for volunteer capability tracking';
