-- Update distributor names based on GlobalId patterns
UPDATE Products 
SET DistributorName = 'TechWorld' 
WHERE GlobalId LIKE 'TWN%';

UPDATE Products 
SET DistributorName = 'ElectroCom' 
WHERE GlobalId LIKE 'EC%';

UPDATE Products 
SET DistributorName = 'GadgetCentral' 
WHERE GlobalId LIKE 'GC%';

-- For products without clear patterns, set to a default distributor
UPDATE Products 
SET DistributorName = 'TechWorld' 
WHERE DistributorName = 'GadgetHub' OR DistributorName IS NULL OR DistributorName = '';

-- Show the results
SELECT Id, GlobalId, Name, DistributorName FROM Products;






