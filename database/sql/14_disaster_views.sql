CREATE OR REPLACE VIEW vw_disaster_summary AS
SELECT
    d.disaster_id,
    d.title,
    d.severity_level,
    d.status,
    total_victims(d.disaster_id) AS victim_count
FROM disasters d;