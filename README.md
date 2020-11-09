README

This was based on the ASP.NET Web API Template in Visual Studio. All Models are our own.

# cpsc-471-project

**Important General Info (Read me first):**

1. In order to run this project, you will need Visual Studio 2019 Community (hereafter referred to as VS). Any recent version should work.
   - You will need the ASP.NET package to run this project
   - After downloading VS and the above package(s), open the cpsc-471-project.sln project/solution in VS
   - Then, in VS, go to Tools > NuGet Package Manager > Manage Nuget Packages For Solution
   - If the packages listed under "ItemGroup" in cpsc-471-project.csproj have not already been installed, then install them.

2. The database is sqlite
- To download debugging tools for sqlite, go to https://www.sqlite.org/download.html (you will likely need this for development)
- Download the command line shell program, sqldiff.exe program, and sqlite3_analyzer.exe program as these will come in handy later
- You may want to download these to the same directory as the location where your sqlite database will be created

3. The databse of this project is named "JobHunterDb.sqlite" and by default, it will be located in the parent folder of the directory in which this README is located.
The location can be changed by changing the following code in cpsc-471-project\\cpsc-471-project\\Startup.cs
> public const string DBLocation = "Data Source='{DESIRED_LOCATION}'"

4. Upon initially opening the project or changing Git branches, you will need to set up the database.
   This should be relatively straighforward, simply follow the "Creating and Upgrading your Database" instructions later in this document.

5. During development, Each time you change any of the models (in cpsc-471-project\\cpsc-471-project\\Models), you will need to create a database migration.
   A "Migration" is essentially an instruction that allows the database to automatically upgrade. Our project will end up having many migrations,
   which will be applied sequentially to create our full, up-to-date database.
   To learn more, go to the "Updating Models and creating Migrations" section.

6. Joel is currently working on automatically adding sample data to ease testing/debugging


**Creating and Upgrading your Database:**
1. Go to Tools > NuGet Package Manager > Package Manager Console, then enter command
   > Update-Database
This will automatically create a database at the "DBLocation = "Data Source='{DESIRED_LOCATION}'" with the current models
IMPORTANT: In the sqlite command line tool , you must run the following commands upon initially creating the database:
> .open JobHunterDB.sqlite
> PRAGMA foreign_keys = ON;
This enables foreign key/referetial integrity constraint enforcement


**Updating Models and creating Migrations**
1. Go to Tools > NuGet Package Manager > Package Manager Console, then enter the commands
   > Add-Migration Helpful Migration Name
   > Update-Database
This will automatically create a migration for your database under cpsc-471-project\\cpsc-471-project\\Migrations (you will need to commit this file to Git)
If you are updating the models/database schema, make sure to communicate with anyone else that is working on the database models, since
working on multiple migrations simutaneously can cause requirement conflicts (and worse, these requirement conflicts do not show up as merge conflicts)

Some notes about Migrations:
- If you want a behaviour upon deletion other than Cascading deletion, then you need to manually go into the migration and change the "onDelete: ReferentialAction.*" to appropriate action 

**Instructions for hitting API Endpoints:**
1. Download the Postman Desktop App
2. Sign in to the app (create an account if you haven't already)
3. Turn off SSL certificate verification via File > Settings > SSL Certificate Verification 
* Since the SSL certificate is local, it can't be verified. make sure to turn this setting back on if you are hitting public endpoints (we won't be for this project))
4. Go to Solution View and run IIS Express (if you aren't already)
5. Enter the desired endpoint in Postman and hit "Send" (sample endpoint: https://localhost:44397/weatherforecast)

Reference for how to create new models:
https://docs.microsoft.com/en-ca/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio


**Troubleshooting:**
If a webpage pops up but you get an error message saying that a table is missing or that a certain field of the table does not exist:
- Your databse schema may be out of data. Follow the **Updating the Database Schema** instructions in this document

If the program is building but not running at all:
If you accidentally click in the cmd windows (including the window that appears when starting this program) it will halt by default (Microsoft is very silly)

If foreign keys not being enforced:
Open the sqlite command tool and run
> .open JobHunterDB.sqlite
> PRAGMA foreign_keys = ON;
Note that this will not enforce foreign key constraints on existing data, so you have to manually go back and fix any existing errors


Relevant links:
Hiding certain fields from items:
https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio#over-post

Entity relationships:
https://docs.microsoft.com/en-us/ef/core/modeling/relationships