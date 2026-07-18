# Final Lab Report: Disaster Information and Emergency Management System (DIEMS)

## 1. Project Overview - Introduction
During natural disasters, organizing displaced people, emergency supplies, and volunteers can become chaotic and overwhelming. The Disaster Information and Emergency Management System (DIEMS) is a centralized, database-driven web application designed to solve this problem. The primary objective of this project was to design a robust relational database from scratch and integrate it into a functional, user-friendly web interface. By relying heavily on advanced database concepts, the system ensures that relief operations are handled efficiently, securely, and without data corruption.

## 2. Tools and Technology
*   **Database Engine:** Oracle Database 11g/19c (SQL and PL/SQL)
*   **Backend Application:** C# utilizing the ASP.NET Core MVC framework
*   **Frontend Interface:** HTML, CSS, JavaScript, and Razor Pages
*   **Database Connectivity:** Oracle Managed Data Access (ADO.NET)
*   **Architecture Pattern:** Model-View-Controller (MVC)

## 3. Features - Elaborately
DIEMS is divided into interconnected modules that work together to manage all aspects of a crisis:
*   **Victim & Shelter Management:** Admins can register displaced victims, record their health status, and assign them to safe shelters. The system automatically calculates and tracks shelter capacities in real-time, preventing overbooking.
*   **Volunteer Operations:** The system maintains a complete directory of volunteers, their skill sets, and current availability. Admins can assign volunteers to specific disaster missions, track their total hours served, and view their active assignments.
*   **Resource Inventory & Distribution:** The system keeps a running total of emergency supplies (such as food, water, and medicine). Whenever supplies are distributed to affected areas or shelters, the system dynamically deducts the amount from the warehouse stock, highlighting critical shortages.
*   **Hospital Records:** A quick-reference directory of nearby hospitals is maintained. It tracks contact information, locations, and available bed capacities for immediate medical emergencies.

## 4. Database Tables
The foundation of the system is built on normalized relational tables (up to 3NF) to ensure strict data integrity:
*   **USERS / ROLES:** Manages authentication and authorization. It securely stores admin and staff credentials and defines their access levels.
*   **VICTIMS:** Stores personal details, health conditions, and current shelter assignments of displaced individuals using foreign keys.
*   **SHELTERS & SHELTER_CAPACITY:** Stores shelter addresses and contact info, while a linked capacity table tracks total capacity, currently occupied beds, and available beds.
*   **VOLUNTEERS & VOLUNTEER_ASSIGNMENTS:** Keeps track of volunteer profiles, skills, and a complete history of tasks or missions they have been assigned to.
*   **RESOURCES & INVENTORY:** Catalogs all emergency supply types (e.g., Food, Medicine). The inventory table logs every distribution event and dynamically tracks remaining stock.
*   **HOSPITALS:** Maintains a directory of medical centers, their contact details, and bed availability for emergency routing.

## 5. Functions and Procedures Explanation
Because this is a database-centric project, complex business logic is executed directly inside the Oracle database rather than the application code:
*   **Stored Procedures (Atomic Operations):** Procedures like `ALLOCATE_SHELTER` are used for complex actions that modify multiple tables. When a victim is assigned a shelter, the procedure checks bed availability, updates the victim's location, and updates the shelter's capacity all at once. This is wrapped in a database **Transaction**—using `COMMIT` if everything succeeds, or `ROLLBACK` if an error occurs (like the shelter running out of space mid-process). This ensures data is never corrupted.
*   **PL/SQL Functions (Data Retrieval):** Functions like `GET_FILTERED_VOLUNTEERS` are used for dynamic searching. When an admin filters the volunteer list on the website, the web app passes the criteria to the function. The function executes the complex filtering logic on the database side and returns exactly the requested data.

## 6. Triggers, Cursors, and DUAL Overview
*   **Database Triggers (Automation):** Triggers act as the automatic nervous system of the project. 
    *   An `INVENTORY_UPDATE_TRG` trigger automatically deducts stock from the `RESOURCES` table whenever supplies are distributed.
    *   A `SHELTER_CAP_UPD_TRG` trigger automatically updates the `AVAILABLE_BEDS` count when a victim checks in or out of a shelter.
    *   An `AUDIT_CHANGES_TRG` silently records a log of who deleted or modified important records, creating a secure administrative history.
*   **Cursors (Dynamic Filtering):** Inside the PL/SQL functions, I utilized `SYS_REFCURSOR`. A cursor allows the database to dynamically join multiple tables, apply complex `WHERE` filters, sort the data, and return a clean, readable result set to the web application efficiently.
*   **Sequences and the DUAL Table (Primary Keys):** Since every new record (like a new victim or volunteer) needs a unique ID, I created Oracle **Sequences**. To fetch the next available number safely before inserting a row, the system runs the query `SELECT sequence_name.NEXTVAL FROM DUAL`. The **DUAL** table is a special one-row Oracle table used specifically to calculate these numbers and timestamps safely.

## 7. System Workflow
1. **Authentication:** An admin or staff member logs into the DIEMS web portal securely.
2. **Data Entry:** As a disaster unfolds, staff register new victims, volunteers, and incoming resource shipments into the system. Oracle sequences automatically assign unique IDs using the DUAL table.
3. **Allocation & Assignment:** Admins assign victims to shelters and deploy volunteers to tasks. The web app calls Stored Procedures, which use Transactions to safely update multiple tables simultaneously.
4. **Automation:** As allocations happen, Database Triggers automatically fire in the background to update shelter bed counts and deduct distributed resources from the central inventory.
5. **Reporting:** When the admin views dashboards or lists, PL/SQL Functions and Cursors execute complex queries in milliseconds, displaying real-time, filtered data on critical inventory shortages and available volunteers.

## 8. Conclusion
Building the Disaster Information and Emergency Management System (DIEMS) was a highly practical learning experience that bridged the gap between theoretical database concepts and real-world application development. By utilizing advanced Oracle features like PL/SQL procedures, functions, cursors, and triggers, the system effectively shifts complex data processing and automation directly into the database layer. This ensures real-time accuracy, high performance, and strict data integrity. Ultimately, this project demonstrates how a well-structured relational database can serve as a reliable backbone for managing critical, time-sensitive crisis operations.
