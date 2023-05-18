using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PICI.Controllers;
using PICI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;


namespace PICI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers()
                  .AddNewtonsoftJson()
                 .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                });
            services.AddTransient<SigninController>();
            services.AddHttpContextAccessor();
            services.AddSession(m => m.IdleTimeout = TimeSpan.FromMinutes(30));
            services.AddHttpClient();
            services.AddScoped<SigninRepository>();
         //   services.AddScoped<AssettypeRepository>();
         //   services.AddScoped<AssetRepository>();
        ///   services.AddScoped<BranchRepository>();
          //  services.AddScoped<CompanyRepository>();
         //   services.AddScoped<DepartmentRepository>();
            services.AddScoped<MenuRepository>();
            services.AddScoped<CustomerRepository>();
            services.AddScoped<ProjTrackerRepository>();
            services.AddScoped<ProjectRepository>();
            services.AddScoped<ServerInfoRepository>();
         //   services.AddScoped<VendorRepository>();
          //  services.AddScoped<StatusRepository>();
          ////  services.AddScoped<RequestRepository>();
            //services.AddScoped<ScrapRepository>();
           // services.AddScoped<LocationRepository>();
          //  services.AddScoped<ReportsRepository>();
          //  services.AddScoped<TransferRepository>();

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PICI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PICI v1"));
            }

            app.UseRouting();
            // global cors policy
            app.UseCors(x => x
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true) // allow any origin
              .AllowCredentials()); // allow credentials
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
