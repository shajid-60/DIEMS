-- ============================================================
-- DIEMS — Create Table: VOLUNTEER_SKILLS
-- Module: Volunteer Management (Junction Table)
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE VOLUNTEER_SKILLS (
    VS_ID        NUMBER PRIMARY KEY,
    VOLUNTEER_ID NUMBER NOT NULL,
    SKILL_ID     NUMBER NOT NULL,
    PROFICIENCY  VARCHAR2(20) DEFAULT 'Intermediate',  -- Beginner, Intermediate, Expert
    CERTIFIED    NUMBER(1)    DEFAULT 0,               -- 1 = has official certification
    CERT_NUMBER  VARCHAR2(100),
    ADDED_AT     TIMESTAMP    DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_VS_VOLUNTEER
        FOREIGN KEY (VOLUNTEER_ID)
        REFERENCES VOLUNTEERS(VOLUNTEER_ID) ON DELETE CASCADE,

    CONSTRAINT FK_VS_SKILL
        FOREIGN KEY (SKILL_ID)
        REFERENCES SKILLS(SKILL_ID) ON DELETE CASCADE,

    CONSTRAINT UQ_VS_VOLUNTEER_SKILL
        UNIQUE (VOLUNTEER_ID, SKILL_ID),

    CONSTRAINT CHK_VS_PROFICIENCY
        CHECK (PROFICIENCY IN ('Beginner', 'Intermediate', 'Advanced', 'Expert')),

    CONSTRAINT CHK_VS_CERTIFIED
        CHECK (CERTIFIED IN (0, 1))
);

CREATE INDEX IDX_VS_VOLUNTEER ON VOLUNTEER_SKILLS(VOLUNTEER_ID);
CREATE INDEX IDX_VS_SKILL     ON VOLUNTEER_SKILLS(SKILL_ID);

COMMENT ON TABLE VOLUNTEER_SKILLS IS 'Many-to-many mapping of volunteers to their skills';
