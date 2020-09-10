# # Text-Em-All Back End Coding Challenge Submission

This is a coding challenge provided by [Text-Em-All](https://www.text-em-all.com/) 
I found the challenge fun and instructional.  

The original challenge was provided here:  https://github.com/callemall/tea-c-sharp-challenge

## Requirements

You will need the following to complete this coding challenge:

1.  Visual Studio 2019
2.  SQL Server 2012 or later
3. .Net Core 3.1+

## Setup

 -  Clone this repository
 -  Using SQL Server, run the  `DBScripts\create_tea_test_db.txt`  script.  This script will create the  `School`  database, set up the schema as show in the [Code Challenge GitHub](https://github.com/callemall/tea-c-sharp-challenge).
 - Using SQL Server, run the `DBScripts\create_tea_test_db.txt`  script to add the constraints required by Challenge #3.

**NUGET PACKAGES**
Each project has Nuget package dependencies that you will need to install as follows:

**School.API Project**
 - AutoMapper v10.0.0
 - AutoMapper.Exentions.Microsoft.DependencyInjection v8.0.1
 - Microsoft.EntityFrameworkCore v3.1.7
 - Microsoft.EntityFrameworkCore.SqlServer v3.1.7
 - **Logging via Serilog**
 - Serilog v2.9.0
 - Serilog.AspNetCore v3.4.0
 - **API Documentation and Test Platform**
 - Swashbuckle.AspNetCore v5.5.1
 - Swashbuckle.AspNetCore.Swagger v5.5.1
 - Swashbuckle.AspNetCore.SwaggerGen v5.5.1
 - Swashbuckle.AspNetCore.SwaggerUI v5.5.1
 - 
**School.Data Class Library Project**
 - AutoMapper v10.0.0
 - AutoMapper.Exentions.Microsoft.DependencyInjection v8.0.1
 - Microsoft.EntityFrameworkCore v3.1.7
 - Microsoft.EntityFrameworkCore.SqlServer v3.1.7


# Running/Testing

As of today, 9/10/2020, I have not finished the testing project.  
You may test the API via the Swagger UI by simply running the School.API project.
That provides a UI that allows you to run the various API based challenges.
