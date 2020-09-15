# # Text-Em-All Back End Coding Challenge Submission

This is a coding challenge provided by [Text-Em-All](https://www.text-em-all.com/) 
I found the challenge fun.   

The original challenge was provided here:  https://github.com/callemall/tea-c-sharp-challenge

## Requirements

You will need the following to complete this coding challenge:

1.  Visual Studio 2019
2.  SQL Server 2012 or later
3. .Net Core 3.1+

## Setup

 -  Clone this repository
 -  Using SQL Server, open a query window, run the  `DBScripts\create_tea_test_db.txt`  script.  This script will create the  `School`  database and set up the schema as shown in the [Code Challenge GitHub](https://github.com/callemall/tea-c-sharp-challenge).
 - Using SQL Server, run the SQL commands found here: `DBScripts\Challenge3Scripts.txt`  (These are part of Challenge #3).

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
 
**School.Data Class Library Project**
 - AutoMapper v10.0.0
 - AutoMapper.Exentions.Microsoft.DependencyInjection v8.0.1
 - Microsoft.EntityFrameworkCore v3.1.7
 - Microsoft.EntityFrameworkCore.SqlServer v3.1.7


# Running/Testing

As of today, 9/11/2020, I have not finished the testing project.  The goal is to provide unit tests for my service layer, the controller/API, and testing the constraints add to the tables inside SQL server. 

You can test the API via the Swagger UI or Postman or Curl.  

## Testing via Swagger
The API's index page loads up the Swagger UI by default.  The Swagger UI allows you to run the various API based challenges.  It runs the commands via Curl.

Or, you can use Postman...

## Testing via Postman
You can import my postman request tests into Postman.  The file is located here:
`Postman\Text-Em-All Code Challenge.postman_collection.json` 

Or, I have Postman Collection/Team you can join which contains the requests.  
https://app.getpostman.com/join-team?invite_code=8b4880893b8a5e1f712eb9f372e17728	
