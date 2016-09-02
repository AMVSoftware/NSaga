-- This script is inspired by Hangfire install.sql: https://github.com/HangfireIO/Hangfire/blob/master/src/Hangfire.SqlServer/Install.sql

PRINT 'Installing NSaga SQL objects...';

BEGIN TRANSACTION;
BEGIN TRY;

/* Start of script *****************************************/

-- Acquire exclusive lock to prevent deadlocks caused by schema creation / version update
DECLARE @SchemaLockResult INT;
EXEC @SchemaLockResult = sp_getapplock @Resource = 'NSaga:SchemaLock', @LockMode = 'Exclusive'

-- Create the database schema if it doesn't exists
IF NOT EXISTS (SELECT [schema_id] FROM [sys].[schemas] WHERE [name] = 'NSaga')
BEGIN
    EXEC (N'CREATE SCHEMA [NSaga]');
    PRINT 'Created database schema [NSaga]';
END
ELSE
    PRINT 'Database schema [NSaga] already exists';


	DECLARE @SCHEMA_ID int;
	SELECT @SCHEMA_ID = [schema_id] FROM [sys].[schemas] WHERE [name] = 'NSaga';


    CREATE TABLE [NSaga].[Sagas] (
        [CorrelationId] [uniqueidentifier] NOT NULL,
        [BlobData] [nvarchar](max) NULL,
        CONSTRAINT [PK_NSaga_Sagas] PRIMARY KEY CLUSTERED ([CorrelationId] ASC)
    );
    PRINT 'Created table [NSaga].[Sagas]';


	CREATE TABLE [NSaga].[Headers](
		[CorrelationId] [uniqueidentifier] NOT NULL,
		[Key] [nvarchar](max) NOT NULL,
		[Value] [nvarchar](max) NULL,
		CONSTRAINT [PK_Headers] PRIMARY KEY CLUSTERED ([CorrelationId] ASC)
	);
	PRINT 'Created table [NSaga].[Headers]';


/* End of script *****************************************/

COMMIT TRANSACTION;
PRINT 'NSagas SQL objects installed';

END TRY
BEGIN CATCH
    DECLARE @ERROR NVARCHAR(MAX);
	SET @ERROR = ERROR_MESSAGE();

	if @@TRANCOUNT > 0
		ROLLBACK TRANSACTION

	RAISERROR(N'NSaga database creation script failed: %s Changes were rolled back, please fix the problem and re-run the script again.', 11, 1, @ERROR);
END CATCH