/* =====================================
   VICTIMS
===================================== */

CREATE TABLE VICTIMS (
    victim_id NUMBER PRIMARY KEY,

    disaster_id NUMBER NOT NULL,

    full_name VARCHAR2(100) NOT NULL,

    gender VARCHAR2(10)
        CHECK (gender IN ('MALE','FEMALE','OTHER')),

    age NUMBER
        CHECK (age >= 0),

    phone VARCHAR2(20),

    medical_condition VARCHAR2(200),

    status VARCHAR2(30)
        DEFAULT 'SAFE'
        CHECK (status IN
        ('SAFE','INJURED','MISSING','DECEASED')),

    CONSTRAINT fk_victim_disaster
        FOREIGN KEY (disaster_id)
        REFERENCES DISASTERS(disaster_id)
);

/* =====================================
   MISSING PERSONS
===================================== */

CREATE TABLE MISSING_PERSONS (
    missing_id NUMBER PRIMARY KEY,

    victim_id NUMBER NOT NULL,

    last_seen_location VARCHAR2(200),

    missing_date DATE,

    found_status VARCHAR2(20)
        DEFAULT 'NOT_FOUND'
        CHECK (found_status IN
        ('FOUND','NOT_FOUND')),

    CONSTRAINT fk_missing_victim
        FOREIGN KEY (victim_id)
        REFERENCES VICTIMS(victim_id)
);