using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace School.API.Extensions
{
    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Text-Em-All School Web API",
                    Description = "Text-Em-All Back End Coding Challenge Designed by Jeff Ogata and Implemented by Clint Carter",

                    TermsOfService = new Uri("https://github.com/callemall/tea-c-sharp-challenge"),
                    Contact = new OpenApiContact
                    {
                        Name = "Clint Carter",
                        Email = "clint@goldencreekrc.com",
                        Url = new Uri("http://github.com/clintcarter1999/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX provided by Text-Em-All's Jeff Ogata",
                        Url = new Uri("https://github.com/callemall/tea-c-sharp-challenge"),
                    }
                });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
