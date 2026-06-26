-- ============================================================
-- DIEMS — Create Table: SHELTER_CAPACITY
-- Module: Shelter Management
-- Oracle 24.3.1
-- NOTE: SHELTER_CAP_TRG fires BEFORE INSERT on SHELTER_RESIDENTS
--       and updates CURRENT_OCCUPIED here
-- ============================================================

CREATE TABLE SHELTER_CAPACITY (
    SC_ID              NUMBER PRIMARY KEY,
    SHELTER_ID         NUMBER        NOT NULL UNIQUE,
    MAX_CAPACITY       NUMBER        NOT NULL,
    CURRENT_OCCUPIED   NUMBER        DEFAULT 0   NOT NULL,
    AVAILABLE_BEDS     NUMBER        DEFAULT 0,
    RESERVED_SPOTS     NUMBER        DEFAULT 0,
    HAS_OVERFLOW       NUMBER(1)     DEFAULT 0,   -- 1 = overflow protocol active
    OVERFLOW_LOCATION  VARCHAR2(200),
    LAST_UPDATED       TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_SC_SHELTER
        FOREIGN KEY (SHELTER_ID)
        REFERENCES SHELTERS(SHELTER_ID) ON DELETE CASCADE,

    CONSTRAINT CHK_SC_CAPACITY
        CHECK (CURRENT_OCCUPIED >= 0 AND MAX_CAPACITY > 0),

    CONSTRAINT CHK_SC_OCCUPIED
        CHECK (CURRENT_OCCUPIED <= MAX_CAPACITY + 10),  -- Allow slight overflow

    CONSTRAINT CHK_SC_OVERFLOW
        CHECK (HAS_OVERFLOW IN (0, 1))
);

COMMENT ON TABLE  SHELTER_CAPACITY                IS 'Real-time capacity tracking for each shelter';
COMMENT ON COLUMN SHELTER_CAPACITY.CURRENT_OCCUPIED IS 'Auto-updated by SHELTER_CAP_TRG on each resident check-in/out';
COMMENT ON COLUMN SHELTER_CAPACITY.AVAILABLE_BEDS   IS 'Computed: MAX_CAPACITY - CURRENT_OCCUPIED - RESERVED_SPOTS';
COMMENT ON COLUMN SHELTER_CAPACITY.HAS_OVERFLOW     IS '1 = shelter is over 95% capacity - overflow protocol activated';
