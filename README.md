# DIEMS — Disaster Intelligence & Emergency Management System

DIEMS is a secure, real-time command center platform designed to coordinate disaster response, emergency medical assistance, and humanitarian relief distribution. Built for government authorities, emergency responders, NGOs, and volunteers.

## Technology Stack
- **Web App**: ASP.NET Core MVC (Targeting .NET 8)
- **Database**: Oracle Database 12c/19c/21c/24c
- **DB Provider**: Oracle.ManagedDataAccess.Core (ODP.NET Managed Driver)
- **Frontend**: HTML5, CSS3, JavaScript, Three.js (interactive 3D globe visualization), Chart.js (analytics rendering)

---

## 1. Oracle Database Installation

The Oracle SQL files are located under `/SQL/` folder. They must be executed in order. A master `install.sql` script is provided to setup the database in one command.

### Installation Steps (using SQL Developer or SQL*Plus):

1. **Create Database User & Grant Permissions**:
   Connect to your database as `SYSTEM` or `SYS` and run:
   ```sql
   CREATE USER DIEMS_USER IDENTIFIED BY your_password;
   GRANT DBA TO DIEMS_USER;
   ```

2. **Run Installation Script**:
   Connect as `DIEMS_USER` and run the master script:
   ```sql
   @install.sql
   ```

### Individual Execution Order (Optional):
If you wish to execute scripts manually, run them in this order:
1. `01_create_tables.sql` — Creates all 28 tables, indexes, constraints, and column comments.
2. `02_create_sequences.sql` — Reference notes on automatic Identity column sequences.
3. `03_create_views.sql` — Creates views for active disasters, available shelters, and critical resource alerts.
4. `04_create_procedures.sql` — Compiles PL/SQL Stored Procedures (`ALLOCATE_SHELTER`, `DISTRIBUTE_RESOURCES`).
5. `05_create_functions.sql` — Compiles PL/SQL Functions (`CALCULATE_DAMAGE`, `AVAILABLE_RESOURCES`, `TOTAL_VICTIMS`).
6. `06_create_triggers.sql` — Compiles PL/SQL Triggers (`INVENTORY_UPDATE_TRG`, `AUDIT_CHANGES_TRG`, `SHELTER_CAP_VAL_TRG`, `SHELTER_CAP_UPD_TRG`, `ALERT_THRESHOLD_TRG`).
7. `07_sample_data.sql` — Seeds realistic, rich test data for testing.

---

## 2. Advanced PL/SQL Logic Features

This project utilizes database-level integrity and business logic using PL/SQL:

- **Automated Shelter Allocation (`ALLOCATE_SHELTER`)**:
  A stored procedure that assigns an evacuated victim to the nearest active shelter that has available capacity (`AVAILABLE_BEDS > 0`).

- **Deductive Resource Distribution (`DISTRIBUTE_RESOURCES` & `INVENTORY_UPDATE_TRG`)**:
  When a relief distribution is inserted into `RESOURCE_DISTRIBUTION`, the `INVENTORY_UPDATE_TRG` trigger automatically subtracts the quantity from the `RESOURCES` inventory table. The `DISTRIBUTE_RESOURCES` stored procedure executes this with stock verification, transactional commit, and logs results in the central audit system.

- **System Change Auditing (`AUDIT_CHANGES_TRG`)**:
  An `AFTER UPDATE` trigger on `DISASTERS` that automatically serializes changes to status, casualties, displaced population, and financial damage estimates, writing them as transactional change records into `AUDIT_LOG`.

- **Capacity Tracking Triggers (`SHELTER_CAP_VAL_TRG` & `SHELTER_CAP_UPD_TRG`)**:
  - `BEFORE INSERT` trigger ensures a shelter cannot exceed its registered capacity.
  - `AFTER INSERT/UPDATE` trigger automatically increments/decrements occupied beds count when people check in or check out of shelters.

- **Threshold Warning Trigger (`ALERT_THRESHOLD_TRG`)**:
  Automatically logs stock warning notifications in the database when available inventory drops below the category's critical threshold percentage.

---

## 3. Web Application Execution

The web app is located under `/DIEMS/` folder.

### Configuration
Update the connection string in `DIEMS/appsettings.json` to match your Oracle Database credentials:
```json
"ConnectionStrings": {
  "OracleDB": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=DIEMS_USER;Password=your_password;"
}
```

### Run Project:
1. Open Visual Studio 2022 and open `DIEMS.slnx` (or `DIEMS.sln`).
2. Alternatively, run using Dotnet CLI in the terminal:
   ```bash
   cd DIEMS
   dotnet run
   ```
3. Navigate to `http://localhost:5000` (or `https://localhost:5001`) in your web browser.

### Seed Login Credentials:
- **Admin**: Username: `admin` | Password: `admin`
- **Official**: Username: `official` | Password: `official`
- **Responder**: Username: `responder` | Password: `responder`
