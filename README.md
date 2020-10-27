README

his was based on the ASP.NET Web API Template in Visual Studio. All Models are our own.

# cpsc-471-project

Instructions for hitting API Endpoints:
1. Download the Postman Desktop App
2. Sign in to the app (create an account if you haven't already)
3. Turn off SSL certificate verification via File > Settings > SSL Certificate Verification 
* Since the SSL certificate is local, it can't be verified. make sure to turn this setting back on if you are hitting public endpoints (we won't be for this project)')
4. Go to Solution View and run IIS Express (if you aren't already)
5. Enter the desired endpoint in Postman and hit "Send" (sample endpoint: https://localhost:44397/weatherforecast)

Reference for how to create new models:
https://docs.microsoft.com/en-ca/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio

Troubleshooting:
- You may have to go to Tools > NuGet Package Manager > Manage NuGet Packages For Solution and install the packages listed under "ItemGroup" in cpsc-471-project.csproj
Current packages:
- version 3.1.9 of Microsoft.EntityFrameworkCore.Sqlite.Core
- version 3.1.9 of Microsoft.EntityFrameworkCore.InMemory
