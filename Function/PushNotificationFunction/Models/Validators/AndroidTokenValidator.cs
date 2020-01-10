using FluentValidation;

namespace PushNotificationFunction.Models.Validators
{
    public class AndroidTokenValidator : AbstractValidator<AndroidToken>
    {
        public AndroidTokenValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UserId).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ApplicationId).GreaterThanOrEqualTo((byte)1);
        }
    }
}
