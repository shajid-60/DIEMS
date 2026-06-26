-- ============================================================
-- DIEMS — Create Table: DISASTER_TYPES
-- Module: Disaster Database
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE DISASTER_TYPES (
    TYPE_ID     NUMBER PRIMARY KEY,
    TYPE_NAME   VARCHAR2(50)  NOT NULL UNIQUE,
    ICON        VARCHAR2(10),             -- Emoji icon e.g. 🌊
    COLOR_CODE  VARCHAR2(10),             -- Hex color for UI
    DESCRIPTION VARCHAR2(300),
    IS_ACTIVE   NUMBER(1) DEFAULT 1 NOT NULL
);


COMMENT ON TABLE DISASTER_TYPES IS 'Reference table for types of disasters tracked in the system';
