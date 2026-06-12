CREATE OR REPLACE PROCEDURE allocate_shelter
(
    p_victim_id  IN NUMBER,
    p_shelter_id IN NUMBER
)
AS
    v_capacity NUMBER;
    v_occupancy NUMBER;
BEGIN

    SELECT capacity,
           current_occupancy
    INTO v_capacity,
         v_occupancy
    FROM shelters
    WHERE shelter_id = p_shelter_id;

    IF v_occupancy >= v_capacity THEN
        RAISE_APPLICATION_ERROR(
            -20001,
            'Shelter is already full.'
        );
    END IF;

    INSERT INTO shelter_assignments
    (
        victim_id,
        shelter_id
    )
    VALUES
    (
        p_victim_id,
        p_shelter_id
    );

    UPDATE shelters
    SET current_occupancy = current_occupancy + 1
    WHERE shelter_id = p_shelter_id;

    COMMIT;

END;
/