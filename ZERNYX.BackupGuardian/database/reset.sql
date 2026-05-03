-- Archivo: database\reset.sql

DELETE FROM backup_history;
DELETE FROM backup_tasks;

UPDATE app_settings
SET
    client_name = 'Cliente',
    technician_name = 'ZERNYX Tech Studio',
    default_backup_destination = '',
    start_minimized = 0,
    start_with_windows = 0,
    enable_notifications = 1,
    enable_scheduler = 1,
    dark_mode = 0,
    retention_days = 30,
    updated_at = datetime('now')
WHERE id = 1;