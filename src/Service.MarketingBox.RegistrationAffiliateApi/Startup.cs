using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Service.MarketingBox.RegistrationAffiliateApi.Grpc;
using Service.MarketingBox.RegistrationAffiliateApi.Modules;
using Service.MarketingBox.RegistrationAffiliateApi.Services;
using Service.MarketingBox.RegistrationAffiliateApi.Swagger;
using SimpleTrading.ServiceStatusReporterConnector;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Service.MarketingBox.RegistrationAffiliateApi
{
    public class Startup
    {
        public Startup()
        {
            ModelStateDictionaryResponseCodes = new HashSet<int>();

            ModelStateDictionaryResponseCodes.Add(StatusCodes.Status400BadRequest);
            ModelStateDictionaryResponseCodes.Add(StatusCodes.Status500InternalServerError);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.BindCodeFirstGrpc();

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddControllers().AddNewtonsoftJson(ConfigureMvcNewtonsoftJsonOptions);
            services.AddSwaggerGen(ConfigureSwaggerGenOptions);
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddMyTelemetry("SP-", Program.Settings.ZipkinUrl);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<HelloService, IHelloService>();

                endpoints.MapGrpcSchemaRegistry();
                
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
            
            app.UseSwagger(c => c.RouteTemplate = "api/{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../../api/v1/swagger.json", "API V1");
                c.RoutePrefix = "swagger/ui";
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<ClientModule>();
        }
        
        protected virtual void ConfigureMvcNewtonsoftJsonOptions(MvcNewtonsoftJsonOptions options)
        {
            var namingStrategy = new CamelCaseNamingStrategy();

            options.SerializerSettings.Converters.Add(new StringEnumConverter(namingStrategy));
            options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = namingStrategy
            };
        }
        
        public ISet<int> ModelStateDictionaryResponseCodes { get; }
        
        protected virtual void ConfigureSwaggerGenOptions(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketingBox.RegistrationAffiliateApi", Version = "v1" });
            options.EnableXmsEnumExtension();
            options.MakeResponseValueTypesRequired();

            foreach (var code in ModelStateDictionaryResponseCodes)
            {
                options.AddModelStateDictionaryResponse(code);
            }

            options.AddJwtBearerAuthorization();
        }
    }
}
