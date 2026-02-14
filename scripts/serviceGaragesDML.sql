-- Parent Services (Main Services)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-101', 'Repair', 1, NULL, 1, NULL, 1, 'All mechanical and electrical repair services', 1, GETDATE(), 'System', 0),
('SRV-102', 'Routine Maintenance', 2, NULL, 1, NULL, 0, 'Standard maintenance services', 1, GETDATE(), 'System', 0),
('SRV-103', 'Body Work', 3, NULL, 1, NULL, 1, 'Vehicle body repairs, dent removal, painting', 1, GETDATE(), 'System', 0),
('SRV-104', 'Overhaul', 4, NULL, 1, NULL, 1, 'Major engine, transmission, or hybrid battery overhauls', 1, GETDATE(), 'System', 0),
('SRV-105', 'Diagnostics', 5, NULL, 1, NULL, 1, 'Fault diagnosis and scanning services', 1, GETDATE(), 'System', 0),
('SRV-106', 'Specialized', 6, NULL, 1, NULL, 1, 'Hybrid/EV services, performance tuning, advanced services', 1, GETDATE(), 'System', 0),
('SRV-107', 'Routine Inspection', 7, NULL, 1, NULL, 0, 'Pre-trip or seasonal basic inspections', 1, GETDATE(), 'System', 0),
('SRV-108', 'Climate & AC Services', 8, NULL, 1, NULL, 1, 'Air conditioning, heating, and climate control services', 1, GETDATE(), 'System', 0),
('SRV-109', 'Tires & Wheels', 9, NULL, 1, NULL, 0, 'Tire replacement, rotation, alignment, balancing', 1, GETDATE(), 'System', 0),
('SRV-110', 'Electrical', 10, NULL, 1, NULL, 1, 'Battery, alternator, starter, and electrical system repairs', 1, GETDATE(), 'System', 0);

-- Sub-services under Repair (ParentId = 101)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-111', 'Engine Repair', 1, 101, 1, NULL, 1, 'Minor or major engine repair', 1, GETDATE(), 'System', 0),
('SRV-112', 'Transmission Repair', 1, 101, 1, NULL, 1, 'Transmission and gearbox repair', 1, GETDATE(), 'System', 0),
('SRV-113', 'Clutch Repair', 1, 101, 1, NULL, 1, 'Clutch replacement or repair', 1, GETDATE(), 'System', 0),
('SRV-114', 'Suspension & Steering Repair', 1, 101, 1, NULL, 1, 'Shocks, struts, steering repair', 1, GETDATE(), 'System', 0),
('SRV-115', 'Brake System Repair', 1, 101, 1, NULL, 1, 'Brake discs, calipers, pads', 1, GETDATE(), 'System', 0),
('SRV-116', 'Exhaust System Repair', 1, 101, 1, NULL, 1, 'Repair or replacement of exhaust system', 1, GETDATE(), 'System', 0);

-- Sub-services under Routine Maintenance (ParentId = 102)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-117', 'Oil & Filter Change', 2, 102, 1, 50.00, 0, 'Standard oil and filter replacement', 1, GETDATE(), 'System', 0),
('SRV-118', 'Tire Rotation', 9, 102, 1, 30.00, 0, 'Rotate tires for even wear', 1, GETDATE(), 'System', 0),
('SRV-119', 'Wheel Alignment', 9, 102, 1, 50.00, 0, 'Adjust wheel alignment', 1, GETDATE(), 'System', 0),
('SRV-120', 'Brake Pad Replacement', 1, 102, 1, 80.00, 0, 'Replace brake pads', 1, GETDATE(), 'System', 0),
('SRV-121', 'Battery Check & Replacement', 10, 102, 1, 120.00, 0, 'Test and replace battery', 1, GETDATE(), 'System', 0),
('SRV-122', 'Fluid Top-up', 2, 102, 1, 20.00, 0, 'Top-up coolant, brake, windshield fluids', 1, GETDATE(), 'System', 0);

-- Sub-services under Body Work (ParentId = 103)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-123', 'Dent / Scratch Removal', 3, 103, 1, NULL, 1, 'Repair small dents and scratches', 1, GETDATE(), 'System', 0),
('SRV-124', 'Panel Replacement', 3, 103, 1, NULL, 1, 'Replace damaged body panels', 1, GETDATE(), 'System', 0),
('SRV-125', 'Full Vehicle Painting', 3, 103, 1, NULL, 1, 'Complete repainting of the vehicle', 1, GETDATE(), 'System', 0);

-- Sub-services under Overhaul (ParentId = 104)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-126', 'Engine Overhaul', 4, 104, 1, NULL, 1, 'Complete engine rebuilding', 1, GETDATE(), 'System', 0),
('SRV-127', 'Transmission Overhaul', 4, 104, 1, NULL, 1, 'Transmission rebuilding', 1, GETDATE(), 'System', 0),
('SRV-128', 'Hybrid Battery Overhaul', 4, 104, 1, NULL, 1, 'High-voltage battery replacement/rebuild', 1, GETDATE(), 'System', 0);

-- Sub-services under Diagnostics (ParentId = 105)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-129', 'OBD / Computer Diagnostics', 5, 105, 1, 100.00, 1, 'Scan ECU and detect faults', 1, GETDATE(), 'System', 0),
('SRV-130', 'Emission / Exhaust Diagnostics', 5, 105, 1, 80.00, 1, 'Check emissions system', 1, GETDATE(), 'System', 0),
('SRV-131', 'Electrical System Diagnosis', 5, 105, 1, 90.00, 1, 'Battery, alternator, wiring checks', 1, GETDATE(), 'System', 0);

-- Sub-services under Specialized (ParentId = 106)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-132', 'Hybrid / EV Battery Diagnostics', 6, 106, 1, NULL, 1, 'Hybrid and EV battery testing', 1, GETDATE(), 'System', 0),
('SRV-133', 'High Voltage Battery Replacement', 6, 106, 1, NULL, 1, 'Replace hybrid or EV battery', 1, GETDATE(), 'System', 0),
('SRV-134', 'Performance Tuning & ECU Remapping', 6, 106, 1, NULL, 1, 'ECU tuning for performance', 1, GETDATE(), 'System', 0);

-- Sub-services under Climate & AC Services (ParentId = 108)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-135', 'AC Repair', 8, 108, 1, 150.00, 1, 'Repair air conditioning system', 1, GETDATE(), 'System', 0),
('SRV-136', 'Heater Repair', 8, 108, 1, 120.00, 1, 'Repair vehicle heater', 1, GETDATE(), 'System', 0),
('SRV-137', 'Climate Control Diagnostics', 8, 108, 1, 100.00, 1, 'Check climate system functionality', 1, GETDATE(), 'System', 0);

-- Sub-services under Tires & Wheels (ParentId = 109)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-138', 'Tire Replacement', 9, 109, 1, 100.00, 0, 'Replace old or damaged tires', 1, GETDATE(), 'System', 0),
('SRV-139', 'Wheel Balancing', 9, 109, 1, 50.00, 0, 'Balance wheels after replacement', 1, GETDATE(), 'System', 0),
('SRV-140', 'Wheel Alignment', 9, 109, 1, 70.00, 0, 'Adjust wheel alignment', 1, GETDATE(), 'System', 0);

-- Sub-services under Electrical (ParentId = 110)
INSERT INTO rpa.GarageServices
(Code, ServiceName, CategoryId, ParentId, ServiceDomainId, EstimatedCost, QuotationRequired, [Description], IsActive, CreatedAt, CreatedBy, IsDeleted)
VALUES
('SRV-141', 'Battery Replacement', 10, 110, 1, 120.00, 1, 'Replace vehicle battery', 1, GETDATE(), 'System', 0),
('SRV-142', 'Starter Motor Repair', 10, 110, 1, 150.00, 1, 'Repair or replace starter motor', 1, GETDATE(), 'System', 0),
('SRV-143', 'Alternator Repair', 10, 110, 1, 200.00, 1, 'Repair or replace alternator', 1, GETDATE(), 'System', 0);
