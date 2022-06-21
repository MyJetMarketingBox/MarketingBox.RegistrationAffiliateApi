using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using AutoWrapper;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.RestApi;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Service;
using Prometheus;
using Service.MarketingBox.RegistrationAffiliateApi.Grpc;
using Service.MarketingBox.RegistrationAffiliateApi.Modules;
using Service.MarketingBox.RegistrationAffiliateApi.Services;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Service.MarketingBox.RegistrationAffiliateApi
{
    public class Startup
    {
        private const string CorsPolicy = "Develop";

        public Startup()
        {
            ModelStateDictionaryResponseCodes = new HashSet<int>();

            ModelStateDictionaryResponseCodes.Add(StatusCodes.Status400BadRequest);
            ModelStateDictionaryResponseCodes.Add(StatusCodes.Status500InternalServerError);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.BindCodeFirstGrpc();
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            // .WithOrigins(
                            //     "http://localhost:3000",
                            //     "http://localhost:3001",
                            //     "http://localhost:3002",
                            //     "http://localhost:3003",
                            //     "http://marketing-box-frontend.marketing-box.svc.cluster.local:3000")
                            .AllowCredentials()
                            .WithHeaders("affiliate-id","api-key")
                            .AllowAnyMethod();
                    });
            });

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddControllers();

            services.SetupSwaggerDocumentation();

            services.AddMyTelemetry("SP-", Program.Settings.ZipkinUrl);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseApiResponseAndExceptionWrapper<ApiResponseMap>(
                new AutoWrapperOptions
                {
                    UseCustomSchema = true,
                    IgnoreWrapForOkRequests = true
                });

            app.UseExceptions();
            
            app.UseRouting();

            app.UseCors(CorsPolicy);

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<HelloService, IHelloService>();

                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapControllers();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });

            app.UseOpenApi(settings => { settings.Path = $"/swagger/api/swagger.json"; });

            app.UseSwaggerUi3(settings =>
            {
                settings.EnableTryItOut = true;
                settings.Path = $"/swagger/api";
                settings.DocumentPath = $"/swagger/api/swagger.json";
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ClientModule>();
        }

        public ISet<int> ModelStateDictionaryResponseCodes { get; }
    }
}