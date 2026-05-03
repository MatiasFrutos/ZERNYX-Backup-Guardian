-- Archivo: database\seed.sql

INSERT OR IGNORE INTO app_settings (
    id,
    client_name,
    technician_name,
    default_backup_destination,
    start_minimized,
    start_with_windows,
    enable_notifications,
    enable_scheduler,
    dark_mode,
    retention_days,
    updated_at
)
VALUES (
    1,
    'Cliente',
    'ZERNYX Tech Studio',
    '',
    0,
    0,
    1,
    1,
    0,
    30,
    datetime('now')
);