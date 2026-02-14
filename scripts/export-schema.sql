-- ============================================================
-- Auto-generated SQL DDL export for database [RepairDb]
-- ============================================================

SET NOCOUNT ON;

-- Generate CREATE TABLE statements for all tables
SELECT 
    'CREATE TABLE [' + s.name + '].[' + t.name + '] (' + CHAR(13) +
    STRING_AGG(
        '    [' + c.name + '] ' +
        TYPE_NAME(c.user_type_id) +
        CASE WHEN c.max_length > 0 AND TYPE_NAME(c.user_type_id) IN ('varchar','nvarchar','char','nchar') 
             THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS NVARCHAR(10)) END + ')' ELSE '' END +
        CASE WHEN c.is_nullable = 0 THEN ' NOT NULL' ELSE ' NULL' END
        , ',' + CHAR(13)
    ) WITHIN GROUP (ORDER BY c.column_id)
    + CHAR(13) + ');' AS CreateTableSQL
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id
JOIN sys.columns c ON t.object_id = c.object_id
GROUP BY s.name, t.name
ORDER BY s.name, t.name;
