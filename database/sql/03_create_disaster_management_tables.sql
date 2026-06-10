/* =====================================
   DISASTERS
===================================== */

CREATE TABLE DISASTERS (
    disaster_id NUMBER PRIMARY KEY,
    disaster_type_id NUMBER NOT NULL,
    district_id NUMBER NOT NULL,

    title VARCHAR2(150) NOT NULL,
    description CLOB,

    severity_level VARCHAR2(20)
        CHECK (severity_level IN
        ('LOW','MEDIUM','HIGH','CRITICAL')),

    start_date DATE NOT NULL,
    end_date DATE,

    status VARCHAR2(20)
        DEFAULT 'ACTIVE'
        CHECK (status IN
        ('ACTIVE','RESOLVED','CLOSED')),

    CONSTRAINT fk_disaster_type
        FOREIGN KEY (disaster_type_id)
        REFERENCES DISASTER_TYPES(disaster_type_id),

    CONSTRAINT fk_disaster_district
        FOREIGN KEY (district_id)
        REFERENCES DISTRICTS(district_id)
);

/* =====================================
   DISASTER UPDATES
===================================== */

CREATE TABLE DISASTER_UPDATES (
    update_id NUMBER PRIMARY KEY,

    disaster_id NUMBER NOT NULL,

    update_text CLOB NOT NULL,

    updated_at DATE DEFAULT SYSDATE,

    CONSTRAINT fk_update_disaster
        FOREIGN KEY (disaster_id)
        REFERENCES DISASTERS(disaster_id)
);

/* =====================================
   DISASTER CASUALTY STATS
===================================== */

CREATE TABLE DISASTER_CASUALTY_STATS (
    stat_id NUMBER PRIMARY KEY,

    disaster_id NUMBER NOT NULL,

    deaths NUMBER DEFAULT 0,
    injured NUMBER DEFAULT 0,
    missing NUMBER DEFAULT 0,

    updated_at DATE DEFAULT SYSDATE,

    CONSTRAINT fk_casualty_disaster
        FOREIGN KEY (disaster_id)
        REFERENCES DISASTERS(disaster_id)
);