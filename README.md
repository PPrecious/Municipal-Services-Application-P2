# Municipal Services Application P2

## 1. Requirements

* Windows 10/11
* Visual Studio 2022
* .NET Framework
* SQL Server / LocalDB

## 2. How to Compile

1. Open the project solution (`.sln`) in Visual Studio.
2. Restore the required NuGet packages.
3. Build the solution by selecting **Build → Build Solution**.
4. Ensure there are no build errors.

## 3. How to Run

1. Set the main project as the **Startup Project**.
2. Press **F5** or select **Start** in Visual Studio.
3. The application will launch.

## 4. How to Use

1. Register or log in to the application.
2. Select the required municipal service.
3. Submit a service request/report.
4. View submitted requests and their status.
5. Administrators can manage requests and update their status.

## 5. Database

Ensure SQL Server/LocalDB is running and that the connection string in the application configuration points to the correct database.

## 6. Troubleshooting

If the application does not run:

* Check the database connection.
* Restore NuGet packages.
* Rebuild the solution.
* Ensure the correct startup project is selected.
