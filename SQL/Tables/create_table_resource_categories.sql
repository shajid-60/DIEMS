-- ============================================================
-- DIEMS — Create Table: RESOURCE_CATEGORIES
-- Module: Emergency Resource Management
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE RESOURCE_CATEGORIES (
    CATEGORY_ID         NUMBER PRIMARY KEY,
    CATEGORY_NAME       VARCHAR2(100) NOT NULL UNIQUE,
    UNIT                VARCHAR2(30)  NOT NULL,   -- packets, liters, pieces, vehicles
    ICON                VARCHAR2(10),
    CRITICAL_THRESHOLD  NUMBER(3)     DEFAULT 30, -- % below which ALERT_THRESHOLD_TRG fires
    DESCRIPTION         VARCHAR2(300),
    IS_ACTIVE           NUMBER(1)     DEFAULT 1 NOT NULL
);


COMMENT ON TABLE  RESOURCE_CATEGORIES                    IS 'Categories of emergency resources tracked by the system';
COMMENT ON COLUMN RESOURCE_CATEGORIES.CRITICAL_THRESHOLD IS 'Stock % below which an alert is triggered automatically';
