# Cache-aside sample implementation

This console app was created to test the Redis Cache for implementing the Cache-aside Cloud Design Pattern.

## Technologies used

### .NET Core 2.0

A .NET Core console application to get information about a Sticker Album. You can _Query data_ to get information about a specific sticker by its number.

### FluentMigrator 

Used to setup the database schema. See class [AddStickerTable.cs](src/console/AddStickerTable.cs).

### Entity Framework Core

Used to query data from the database

### Redis Cache

The Distributed Cache technology used to implement this sample.

## Prepare for running

### Running the ARM Template

Before starting to run the console app, you need to provision the cloud infrastructure needed to run this application. Take a look at the [ARM Template](src/armtemplate/Readme.md) for more information.

### Insert some data into the database

You might need to add some data to the Stickers table before starting. The table schema looks like below. I would add some data like this:

|Id|Number|PlayerName|Country|
|--|------|----------|-------|
|1|370|Gabriel Jesus|Brazil|
|2|371|Neymar Jr.|Brazil|

Tip: use the [Query Editor (Preview)](https://azure.microsoft.com/pt-br/blog/t-sql-query-editor-in-browser-azure-portal/) to make things easier.

### Update the appsettings.json with connection strings

The last step before running this sample is to update [appsettings.json](src/console/appsettings.json) with your connection strings for both the Azure SQL Database and the Redis Cache. To take these info from these workloads, take a look at the documentation below:

* [Azure SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-connect-query-dotnet-visual-studio#for-adonet)
* [Azure Redis Cache](https://docs.microsoft.com/pt-br/azure/redis-cache/cache-dotnet-how-to-use-azure-redis-cache#retrieve-host-name-ports-and-access-keys-using-the-azure-portal)

**Attention!** Take a look at the default username and password for the SQL Server. You're going to need it in your connection string. Change it on [template.json](src/armtemplate/template.json) if you want to, before running the Azure Deployment.

## Running the sample

On the _console_ folder, `dotnet restore` and then `dotnet run` and voilá!