using AIN.PAAS.SQL.Helper.Wrapper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Repository.IRepository;
using AIN.PAAS.SQL.Repository.Repository;
using AIN.PAAS.SQL.Services.IServices;
using AIN.PAAS.SQL.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AIN.PAAS.SQL.API", Version = "v1" });
            });
            string SqlConnection = Environment.GetEnvironmentVariable("ConnectionStrings");

            services.AddDbContext<AINDatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Database"), o =>
            {
                o.EnableRetryOnFailure();
            }));
            services.Configure<DatabaseSettings>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<RegionConfigs>(Configuration.GetSection("RegionConfigs"));
            services.AddTransient<IHospitalService, HospitalService>();
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<ISitesService, SitesService>();
            services.AddTransient<ISitesRepository, SitesRepository>();
            services.AddTransient<ILabsService, LabsService>();
            services.AddTransient<ILabsRepository, LabsRepository>();
            services.AddTransient<ILocationsServices, LocationsServices>();
            services.AddTransient<ILocationsRepository, LocationsRepository>();
            services.AddTransient<IStorageServices, StorageServices>();
            services.AddTransient<IStorageRepository, StorageRepository>();
            services.AddTransient<IInventoryServices, InventoryServices>();
            services.AddTransient<IInventoryRepository, InventoryRepository>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIN.PAAS.API v1"));

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
