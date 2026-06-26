-- ============================================================
-- DIEMS — Create Table: REPORT_ATTACHMENTS
-- Module: Incident Reporting
-- Oracle 24.3.1
-- ============================================================

CREATE TABLE REPORT_ATTACHMENTS (
    ATTACHMENT_ID  NUMBER PRIMARY KEY,
    REPORT_ID      NUMBER        NOT NULL,
    FILE_NAME      VARCHAR2(255) NOT NULL,
    FILE_PATH      VARCHAR2(600) NOT NULL,   -- Relative path in wwwroot/uploads/
    FILE_TYPE      VARCHAR2(50),             -- image/jpeg, video/mp4, application/pdf
    FILE_SIZE_KB   NUMBER,                   -- File size in kilobytes
    DESCRIPTION    VARCHAR2(300),
    IS_EVIDENCE    NUMBER(1)     DEFAULT 1,  -- 1 = primary evidence photo/video
    UPLOADED_BY    NUMBER,
    UPLOADED_AT    TIMESTAMP     DEFAULT SYSTIMESTAMP,

    CONSTRAINT FK_ATT_REPORT
        FOREIGN KEY (REPORT_ID)
        REFERENCES INCIDENT_REPORTS(REPORT_ID) ON DELETE CASCADE,

    CONSTRAINT FK_ATT_UPLOADED_BY
        FOREIGN KEY (UPLOADED_BY)
        REFERENCES USERS(USER_ID),

    CONSTRAINT CHK_ATT_EVIDENCE
        CHECK (IS_EVIDENCE IN (0, 1))
);

CREATE INDEX IDX_ATT_REPORT ON REPORT_ATTACHMENTS(REPORT_ID);

COMMENT ON TABLE  REPORT_ATTACHMENTS            IS 'Photo, video, and document attachments for incident reports';
COMMENT ON COLUMN REPORT_ATTACHMENTS.FILE_PATH  IS 'Server-side relative path: uploads/reports/YYYY/MM/filename.ext';
COMMENT ON COLUMN REPORT_ATTACHMENTS.IS_EVIDENCE IS '1 = marked as key evidence for verification';
