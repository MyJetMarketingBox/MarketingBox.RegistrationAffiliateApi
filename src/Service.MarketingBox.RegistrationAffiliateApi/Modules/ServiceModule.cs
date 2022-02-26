using Autofac;
using FluentValidation;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;
using Service.MarketingBox.RegistrationAffiliateApi.Validators;

namespace Service.MarketingBox.RegistrationAffiliateApi.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<RegistrationRequestValidator>()
                .As<IValidator<RegistrationRequest>>()
                .SingleInstance();
        }
    }
}