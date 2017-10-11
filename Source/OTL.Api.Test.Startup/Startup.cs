using System.Text;
using Exceptionless;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using OTL.Package.Middleware.Extensions;
namespace OTL.Api.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;


        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors(x => x.AddPolicy("OTLCors", y => y.AllowCredentials().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            services.AddEntityFrameworkSqlServer();


            var jwtAuthKey = Configuration.GetValue<string>("JwtServiceConfiguration:PrivateKey");
            var jwtIssuer = Configuration.GetValue<string>("JwtServiceConfiguration:Issuer");

            services.UseOTLAuthentication(jwtIssuer, jwtAuthKey);
            services.UseOTLAuthorization();
            ConfigureLogging(services);
            ConfigureDataLayer(services);
            ConfigureServiceLayer(services);
            
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info() { Title = "OTL.Api.Test", Version = "v1" });
                s.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Copy Bearer {JWT} to the ApiKey Field",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"

                });
            });
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            string exceptionlessApiKey = Configuration.GetValue<string>("Exceptionless:ApiKey");
            ExceptionlessClient.Default.Configuration.ApiKey = exceptionlessApiKey;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Exceptionless()
                .CreateLogger();

            services.AddSingleton(Log.Logger);
        }

        private void ConfigureServiceLayer(IServiceCollection services)
        {
            services.AddLogging();
        }

        private void ConfigureDataLayer(IServiceCollection services)
        {
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "OTL.Api.Test");
                });
            }

            app.UseExceptionHandler("/error/exceptionhandler");
            app.UseMvc();
            app.UseCors("OTLCors");

            loggerFactory.AddSerilog();
        }
    }
}
