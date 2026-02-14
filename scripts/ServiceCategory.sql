INSERT INTO rpa.ServiceCategory (Code, CategoryName, Description, IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('CAT-001', 'Repair', 'Mechanical and electrical repair services that usually require quotation', 1, GETDATE(), 'System', 0),
('CAT-002', 'Routine', 'Regular maintenance services like oil changes and tire rotation, usually no quotation needed', 1, GETDATE(), 'System', 0),
('CAT-003', 'Body Work', 'Vehicle body repairs, dent removal, painting, panel replacement', 1, GETDATE(), 'System', 0),
('CAT-004', 'Overhaul', 'Major engine, transmission, or hybrid battery overhauls, requires quotation', 1, GETDATE(), 'System', 0),
('CAT-005', 'Diagnostics', 'Fault diagnosis, electronic scanning, emission checks, requires quotation', 1, GETDATE(), 'System', 0),
('CAT-006', 'Specialized', 'Hybrid/EV services, performance tuning, and advanced technical services', 1, GETDATE(), 'System', 0),
('CAT-007', 'Routine Inspection', 'Pre-trip or seasonal inspection checks, basic inspections', 1, GETDATE(), 'System', 0),
('CAT-008', 'Climate & AC Services', 'Air conditioning, heater, and climate control repairs', 1, GETDATE(), 'System', 0),
('CAT-009', 'Tires & Wheels', 'Tire replacement, rotation, alignment, and balancing', 1, GETDATE(), 'System', 0),
('CAT-010', 'Electrical', 'Battery, alternator, starter, and vehicle electrical system repairs', 1, GETDATE(), 'System', 0);
