CREATE TABLE ROLES (
    role_id NUMBER PRIMARY KEY,
    role_name VARCHAR2(50) UNIQUE NOT NULL,
    description VARCHAR2(200)
);

CREATE TABLE DIVISIONS (
    division_id NUMBER PRIMARY KEY,
    division_name VARCHAR2(50) UNIQUE NOT NULL
);

CREATE TABLE DISTRICTS (
    district_id NUMBER PRIMARY KEY,
    division_id NUMBER NOT NULL,
    district_name VARCHAR2(50) NOT NULL,

    CONSTRAINT fk_district_division
    FOREIGN KEY (division_id)
    REFERENCES DIVISIONS(division_id)
);

CREATE TABLE UPAZILAS (
    upazila_id NUMBER PRIMARY KEY,
    district_id NUMBER NOT NULL,
    upazila_name VARCHAR2(50) NOT NULL,

    CONSTRAINT fk_upazila_district
    FOREIGN KEY (district_id)
    REFERENCES DISTRICTS(district_id)
);

CREATE TABLE DISASTER_TYPES (
    disaster_type_id NUMBER PRIMARY KEY,
    type_name VARCHAR2(50) UNIQUE NOT NULL,
    description VARCHAR2(300)
);