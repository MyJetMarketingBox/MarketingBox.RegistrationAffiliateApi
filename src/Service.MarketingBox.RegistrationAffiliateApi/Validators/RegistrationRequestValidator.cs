using FluentValidation;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty();
            RuleFor(x => x.Email)
                .NotEmpty();
            RuleFor(x => x.LandingUrl)
                .NotEmpty();
        }
    }
}