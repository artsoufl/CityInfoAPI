using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfoAPI.Contexts;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CityInfoAPI
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter())); // in order to be able to support xml
                 // the followin will make the json object names to start with capital letter. serializer settings
                 // AddJsonOptions(o =>
                 //{
                 //    if (o.SerializerSettings.ContractResolver != null)
                 //    {
                 //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                 //        castedResolver.NamingStrategy = null;
                 //    }
                 //});

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>(); // adding my mail service
#else
            services.AddTransient<IMailService, CloudMailService>(); 
#endif

            // if in launchSettings IISExpress profile, the environment is set to production then the connection string from in there will be used
            // otherwise it will use the one in appsettings
            var connectionString = _config["connectionStrings:cityInfoDBConnectionString"];
                //@"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";
            services.AddDbContext<CityInfoContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            // created once per request
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages(); // optional so I can see the status codes

            app.UseMvc();
        }
    }
}
