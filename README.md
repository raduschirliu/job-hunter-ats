This was based on the <span>ASP.NET</span> Web API Template in Visual Studio. All Models are our own.  
  
# cpsc-471-project

### Important General Info (Read me first)
1. In order to run this project, you will need Visual Studio 2019 Community (hereafter referred to as VS). Any recent version should work.
   - You will need the <span>ASP.NET</span> package to run this project
   - After downloading VS and the above package(s), open the cpsc-471-project.sln project/solution in VS
   - Then, in VS, go to Tools > NuGet Package Manager > Manage Nuget Packages For Solution
   - If the packages listed under "ItemGroup" in cpsc-471-project.csproj have not already been installed, then install them.

2. To properly run the frontend, you will also need a recent version of [NodeJS](https://nodejs.org/en/) installed. When debugging from within VS, all required
dependencies _should_ be automatically installed.
   - The frontend is developed using [NodeJS](https://nodejs.org/en/), [React](https://reactjs.org/), and [TypeScript](https://www.typescriptlang.org/).
   - The [Material-UI library](https://material-ui.com/) was also used extensively for its input components to build the forms.
   - The [React Hook Form](https://react-hook-form.com/) library was used to allow for easy development of forms using React's functional component pattern.
   - [Axios](https://www.npmjs.com/package/axios) was used to make all HTTP requests to the backend.

3. The database is sqlite
   - To download debugging tools for sqlite, go to https://www.sqlite.org/download.html (you will likely need this for development)
   - Download the command line shell program, sqldiff.exe program, and sqlite3_analyzer.exe program as these will come in handy later
   - You may want to download these to the same directory as the location where your sqlite database will be created

4. The database of this project is named `"JobHunterDb.sqlite"` and by default, it will be located in the root directory of the project.
The location can be changed by changing the following code in `cpsc-471-project\cpsc-471-project\Startup.cs`
   ```cs
   public const string DBLocation = "Data Source='{DESIRED_LOCATION}'"
   ```

5. Upon initially opening the project or changing Git branches, you will need to set up the database.  
   - This should be relatively straighforward, simply follow the [Creating and Upgrading your Database](#Creating-and-Upgrading-your-Database) instructions later in this document.

6. During development, each time you change any of the models (in `cpsc-471-project\cpsc-471-project\Models`), you will need to create a database migration.  
   - A "Migration" is essentially an instruction that allows the database to automatically upgrade. Our project will end up having many migrations,
   which will be applied sequentially to create our full, up-to-date database.
   To learn more, go to the [Updating Models and creating Migrations](#Updating-Models-and-Creating-Migrations) section.

7. Some sample data already exists in the `SampleData.cs` file in the `cpsc-471-project\cpsc-471-project\Test` directory, visit that file for an easy way to add sample data.

### Running the Frontend
- In order for the frontend to run, a recent version of NodeJS should be installed.
- Upon debugging the project from within VS, the frontend should also be automatically started and hosted at http://localhost:5001.
- If VS fails to install npm dependencies or fails with an error relating to `react-scripts` not existing, open a terminal within the
`cpsc-471-project/ClientApp` folder, and run the following command to manually update npm dependencies
   ```bash
   npm install
   ```
- The frontend requires the backend REST API in order to properly function, and will throw 404 errors if it is not found when making requests.
- The backend should be running locally on the same machine as the frontend.

### Creating and Upgrading your Database
- Go to Tools > NuGet Package Manager > Package Manager Console, then enter command
   ```bash
   > Update-Database
   ```
   This will automatically create a database at the `"DBLocation = "Data Source='{DESIRED_LOCATION}'"` with the current models
- _Note_: In the sqlite command line tool , you must run the following commands upon initially creating the database:
   ```bash
   > .open JobHunterDB.sqlite
   > PRAGMA foreign_keys = ON;
   ```
   This enables foreign key/referential integrity constraint enforcement


### Updating Models and Creating Migrations
- Go to Tools > NuGet Package Manager > Package Manager Console, then enter the commands  
   ```bash
   > Add-Migration Helpful-Migration-Name
   > Update-Database
   ```
- This will automatically create a migration for your database under `cpsc-471-project\cpsc-471-project\Migrations` (you will need to commit this file to Git)
- If you are updating the models/database schema, make sure to communicate with anyone else that is working on the database models, since
working on multiple migrations simutaneously can cause requirement conflicts (and worse, these requirement conflicts do not show up as merge conflicts)

_Some notes about Migrations:_
   - If you want a behaviour upon deletion other than Cascading deletion, then you need to manually go into the migration and change the `"onDelete: ReferentialAction.*"` to appropriate action 

[Reference for how to create new models](https://docs.microsoft.com/en-ca/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)

### Seeding the Database
- There is a built-in route for seeding the database with dummy data. This route is only available when running the project in `debug` mode in VS.
- To seed the DB, first start the API in `debug` mode.
- Next, send a `POST` request to the following endpoint: `https://localhost:5001/api/users/populatedb`.
- If there are no entries in the DB, sample data will be added

### Instructions for hitting API Endpoints
1. Download the Postman Desktop App
2. Sign in to the app (create an account if you haven't already)
3. Turn off SSL certificate verification via File > Settings > SSL Certificate Verification 
   - Since the SSL certificate is local, it can't be verified. make sure to turn this setting back on if you are hitting public endpoints (we won't be for this project)
4. Run cpsc_471_project using the play button in the 2nd toolbar (beside "Any CPU"). If the button is not visible, you may have to switch to Solution View in the Solution Explorer
5. Enter the desired endpoint URL in Postman and add the authorization info (more info in the [Authorizing Requests](#Authorizing-Requests) section)
6. Enter the relevant parameters in the body section of Postman. For example, the POST endpoint `https://localhost:5001/api/companies` requires information in the following form
```json
{
    "companyid": 1,
    "name": "Test Company 1",
    "adminid": "admin-user"
}
```
7. Hit Send!
   - Note that since SSL certificate is turned off, you should see a "Warning: Unable to verify the first certificate" that you can ignore during development if the server is on your local machine (obviously don't ignore this if you are hitting non-local endpoints though)

### Authorizing Requests
1. Upon startup, three roles will be created: Admin, Recruiter, and User
    (1.1. You can add some sample users to the roles via the instructions in the file in the `cpsc-471-project\cpsc-471-project\Test\SampleData.cs` files)
2. We are authenticating requests using Bearer Tokens. To get a bearer token, hit the endpoint `api/auth/login` with a request of the form:
```json
{
    "username": "yourusername",
    "password": "yourpassword"
}
```
3. The endpoint will return a Bearer Token and its expiration date.
4. To use the bearer token in Postman, go to Authorization and select Bearer Token, then paste the bearer token
5. You can then make an API call as described in line 5 and beyond in [Updating the Database Schema](#Instructions-for-hitting-API-Endpoints)


### Adding New Users
1. To add a new user, use the `api/auth/register` endpoint via Postman
2. The body of your request should be of the form:
```json
{
    "username": "newusername",
    "password": "newpassword",
    "firstname": "newfirstname",
    "lastname": "newlastname"
}
```
3. The new user will automatically be created with the candidate role and a Bearer Token will be returned.

Note that there are several other functions in `AuthController.cs` that allow you to change user roles, test user validity, etc.


### Troubleshooting
- If a webpage pops up but you get an error message saying that a table is missing or that a certain field of the table does not exist:
   - Your database schema may be out of data. Follow the [Updating the Database Schema](#Creating-and-Upgrading-your-Database) instructions in this document

- If the program is building but not running at all:  
   - If you accidentally click in the cmd prompt (including the window that appears when starting this program) it will halt by default (Microsoft is very silly)

- If foreign keys not being enforced:
   - Open the sqlite command tool and run
      ```shell
      > .open JobHunterDB.sqlite
      > PRAGMA foreign_keys = ON;
      ```
      _Note:_ this will not enforce foreign key constraints on existing data, so you have to manually go back and fix any existing errors


### Relevant links
- [Hiding/protecting certain fields from items](
https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio#over-post)

- [Entity relationships info](
https://docs.microsoft.com/en-us/ef/core/modeling/relationships)

- Authentication in <span>ASP.NET</span> Web API
   - https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/authentication-and-authorization-in-aspnet-web-api
   - https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api

- [Security if we decide to add a UI](
https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/preventing-cross-site-request-forgery-csrf-attacks)