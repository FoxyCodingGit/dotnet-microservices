using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env; // added IWebHostEnvironment to see if its prod or debug mode, no extra stuff behind the scenes so can add to constructor without additional logic.
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsProduction())
            {
                Console.WriteLine("--> Using SqlServer Db");
                // Add using sql service when in production (in docker image hosted in Kubernetes pod)
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn"))); // as called it ConnectionStrings in appsettings, can use "GetConnectionString" for delicious readability
            }
            else
            {
                Console.WriteLine("--> Using InMem Db");
                services.AddDbContext<AppDbContext>(opt => 
                    opt.UseInMemoryDatabase("InMem"));
            }

            services.AddScoped<IPlatformRepo, PlatformRepo>();

            // Learning-notes - client factory, when HttpCommandDataClient constructor called going to inject the httpClient. The factory manages this.
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>(); 
            services.AddSingleton<IMessageBusClient, MessageBusClient>(); // singleton as we want just one connection being used everywhere in the application.
            
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            // learning-notes - Simple console line that shows commandservice endpoint when running dotnet run, simple but good show of info.
            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}
