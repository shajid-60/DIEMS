/* =====================================
   SHELTERS
===================================== */

CREATE TABLE SHELTERS (
    shelter_id NUMBER PRIMARY KEY,

    district_id NUMBER NOT NULL,

    shelter_name VARCHAR2(100) NOT NULL,

    capacity NUMBER NOT NULL
        CHECK (capacity > 0),

    current_occupancy NUMBER DEFAULT 0
        CHECK (current_occupancy >= 0),

    contact_number VARCHAR2(20),

    CONSTRAINT fk_shelter_district
        FOREIGN KEY (district_id)
        REFERENCES DISTRICTS(district_id)
);

/* =====================================
   SHELTER ASSIGNMENTS
===================================== */

CREATE TABLE SHELTER_ASSIGNMENTS (
    assignment_id NUMBER PRIMARY KEY,

    victim_id NUMBER NOT NULL,

    shelter_id NUMBER NOT NULL,

    assigned_date DATE DEFAULT SYSDATE,

    CONSTRAINT fk_assignment_victim
        FOREIGN KEY (victim_id)
        REFERENCES VICTIMS(victim_id),

    CONSTRAINT fk_assignment_shelter
        FOREIGN KEY (shelter_id)
        REFERENCES SHELTERS(shelter_id)
);