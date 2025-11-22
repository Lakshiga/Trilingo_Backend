-- =============================================
-- CLEAR ALL DATA FROM TRILINGO DATABASE
-- =============================================
-- WARNING: This script will DELETE ALL DATA from the database!
-- Make sure you have a backup before running this script.
-- =============================================
-- Database: Trilingo_Learning_Db
-- Server: trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com
-- =============================================

USE [Trilingo_Learning_Db];
GO

-- Disable foreign key constraints temporarily for easier deletion
-- (We'll delete in order, but this helps if there are circular dependencies)

BEGIN TRANSACTION;

BEGIN TRY
    -- =============================================
    -- DELETE IN ORDER (Respecting Foreign Key Constraints)
    -- =============================================
    
    -- 1. Delete Student Progresses (depends on Activities and Students)
    PRINT 'Deleting Student Progresses...';
    DELETE FROM [StudentProgresses];
    PRINT 'Student Progresses deleted.';
    
    -- 2. Delete Exercises (depends on Activities)
    PRINT 'Deleting Exercises...';
    DELETE FROM [Exercises];
    PRINT 'Exercises deleted.';
    
    -- 3. Delete Activities (depends on ActivityTypes, MainActivities, Stages)
    PRINT 'Deleting Activities...';
    DELETE FROM [Activities];
    PRINT 'Activities deleted.';
    
    -- 4. Delete Activity Types (depends on MainActivities)
    PRINT 'Deleting Activity Types...';
    DELETE FROM [ActivityTypes];
    PRINT 'Activity Types deleted.';
    
    -- 5. Delete Main Activities
    PRINT 'Deleting Main Activities...';
    DELETE FROM [MainActivities];
    PRINT 'Main Activities deleted.';
    
    -- 6. Delete Stages (depends on Levels)
    PRINT 'Deleting Stages...';
    DELETE FROM [Stages];
    PRINT 'Stages deleted.';
    
    -- 7. Delete Levels (depends on Languages)
    PRINT 'Deleting Levels...';
    DELETE FROM [Levels];
    PRINT 'Levels deleted.';
    
    -- 8. Delete Languages
    PRINT 'Deleting Languages...';
    DELETE FROM [Languages];
    PRINT 'Languages deleted.';
    
    -- 9. Delete Students (depends on Users)
    PRINT 'Deleting Students...';
    DELETE FROM [Students];
    PRINT 'Students deleted.';
    
    -- 10. Delete Admins (depends on Users)
    PRINT 'Deleting Admins...';
    DELETE FROM [Admins];
    PRINT 'Admins deleted.';
    
    -- 11. Delete Users (depends on Roles)
    PRINT 'Deleting Users...';
    DELETE FROM [Users];
    PRINT 'Users deleted.';
    
    -- 12. Delete Roles (if you want to clear roles too)
    -- Uncomment the next 3 lines if you want to delete roles as well
    -- PRINT 'Deleting Roles...';
    -- DELETE FROM [Roles];
    -- PRINT 'Roles deleted.';
    
    -- =============================================
    -- RESET IDENTITY COLUMNS (Optional)
    -- =============================================
    -- This resets the auto-increment counters to start from 1 again
    -- Uncomment if you want to reset ID counters
    
    -- DBCC CHECKIDENT ('[Activities]', RESEED, 0);
    -- DBCC CHECKIDENT ('[ActivityTypes]', RESEED, 0);
    -- DBCC CHECKIDENT ('[MainActivities]', RESEED, 0);
    -- DBCC CHECKIDENT ('[Stages]', RESEED, 0);
    -- DBCC CHECKIDENT ('[Levels]', RESEED, 0);
    -- DBCC CHECKIDENT ('[Languages]', RESEED, 0);
    -- DBCC CHECKIDENT ('[StudentProgresses]', RESEED, 0);
    -- DBCC CHECKIDENT ('[Exercises]', RESEED, 0);
    
    COMMIT TRANSACTION;
    
    PRINT '=============================================';
    PRINT 'SUCCESS: All data has been deleted!';
    PRINT '=============================================';
    PRINT 'You can now use your admin panel to add:';
    PRINT '1. Main Activities';
    PRINT '2. Activity Types';
    PRINT '3. Other data as needed';
    PRINT '=============================================';
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '=============================================';
    PRINT 'ERROR: Transaction rolled back!';
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR);
    PRINT '=============================================';
    
    THROW;
END CATCH;
GO

-- =============================================
-- VERIFICATION QUERIES (Optional - Run after deletion)
-- =============================================
-- Uncomment to verify all tables are empty

-- SELECT 'Activities' AS TableName, COUNT(*) AS RowCount FROM [Activities]
-- UNION ALL
-- SELECT 'ActivityTypes', COUNT(*) FROM [ActivityTypes]
-- UNION ALL
-- SELECT 'MainActivities', COUNT(*) FROM [MainActivities]
-- UNION ALL
-- SELECT 'Stages', COUNT(*) FROM [Stages]
-- UNION ALL
-- SELECT 'Levels', COUNT(*) FROM [Levels]
-- UNION ALL
-- SELECT 'Languages', COUNT(*) FROM [Languages]
-- UNION ALL
-- SELECT 'Exercises', COUNT(*) FROM [Exercises]
-- UNION ALL
-- SELECT 'StudentProgresses', COUNT(*) FROM [StudentProgresses]
-- UNION ALL
-- SELECT 'Students', COUNT(*) FROM [Students]
-- UNION ALL
-- SELECT 'Admins', COUNT(*) FROM [Admins]
-- UNION ALL
-- SELECT 'Users', COUNT(*) FROM [Users];

