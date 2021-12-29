using Microsoft.Extensions.DependencyInjection;
using NSwag;
using System.Linq;

namespace Service.MarketingBox.RegistrationAffiliateApi
{
    public static class StartupUtils
    {
        public static void SetupSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerDocument(o =>
            {
                o.Title = "Affiliate API";
                o.GenerateEnumMappingDescription = true;

                //o.AddSecurity("Bearer", Enumerable.Empty<string>(),
                //    new OpenApiSecurityScheme
                //    {
                //        Type = OpenApiSecuritySchemeType.ApiKey,
                //        Description = "Bearer Token",
                //        In = OpenApiSecurityApiKeyLocation.Header,
                //        Name = "Authorization"
                //    });
            });
        }
    }
}
