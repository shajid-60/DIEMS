-- ============================================================
-- DIEMS — Dynamic Clean-up Script
-- Oracle 11g Compatibility
--
-- PURPOSE:
--   Dynamically drops all existing objects (tables, views,
--   procedures, functions, sequences, triggers) in the current 
--   schema to allow a completely clean installation.
-- ============================================================

SET SERVEROUTPUT ON;
PROMPT Cleaning up existing database objects...

BEGIN
  FOR cur_rec IN (SELECT object_name, object_type
                  FROM user_objects
                  WHERE object_type IN
                             ('TABLE',
                              'VIEW',
                              'MATERIALIZED VIEW',
                              'PACKAGE',
                              'PROCEDURE',
                              'FUNCTION',
                              'SEQUENCE',
                              'SYNONYM',
                              'PACKAGE BODY',
                              'TRIGGER',
                              'TYPE'
                             )
                  -- Ensure we drop tables last (after types and views etc)
                  -- or handle cascading. Tables with CASCADE CONSTRAINTS handles most things.
                  ORDER BY CASE object_type
                           WHEN 'TRIGGER' THEN 1
                           WHEN 'PROCEDURE' THEN 2
                           WHEN 'FUNCTION' THEN 3
                           WHEN 'PACKAGE BODY' THEN 4
                           WHEN 'PACKAGE' THEN 5
                           WHEN 'VIEW' THEN 6
                           WHEN 'MATERIALIZED VIEW' THEN 7
                           WHEN 'SYNONYM' THEN 8
                           WHEN 'SEQUENCE' THEN 9
                           WHEN 'TABLE' THEN 10
                           WHEN 'TYPE' THEN 11
                           ELSE 99 END)
  LOOP
    BEGIN
      IF cur_rec.object_type = 'TABLE' THEN
        EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' "' || cur_rec.object_name || '" CASCADE CONSTRAINTS';
      ELSE
        EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' "' || cur_rec.object_name || '"';
      END IF;
    EXCEPTION
      WHEN OTHERS THEN
        DBMS_OUTPUT.put_line('Failed to drop ' || cur_rec.object_type || ' ' || cur_rec.object_name || ': ' || SQLERRM);
    END;
  END LOOP;
END;
/

-- Drop any remaining database links if applicable (optional, usually not needed)
-- Purge recycle bin to free up space and names
PURGE RECYCLEBIN;

PROMPT Schema cleanup complete.
