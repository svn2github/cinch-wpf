This project is uses a LINQ to SQL class, and as such is expecting a SQL database to be present.

So you will need to do the following prior to running the app

1. Setup the database against your own SQL server installation, this can be done by running the
   CREATE THE DATABASE.sql SQL script. The database should be called "MVVM_Demo"
2. Create the DB structure, this can be done by  running the
   CREATE THE TABLES.sql SQL script, against the newly created "MVVM_Demo" database from step1
3. Certain static data is expected to exist in the MVVM_Demo database. This can be setup by
   running the ADD PRODUCTS.sql SQL script. This will add ALL required static data

*******************************************************
IMPORTANT NOTE
*******************************************************
4. Do not forget to change the connection string in the App.Config of the
   MVVM.DataAccess project to point to YOUR own SQL server instance.
5. Once you have done that make sure to open the Properties.Settings.settings file in design mode
   (basically double click the file, and it will open in design mode in Visual Studio)
   which will prompt you to overrwite the setting as the appear in the download. Accept that. Which
   will change the settings to use your new sql connection string.
