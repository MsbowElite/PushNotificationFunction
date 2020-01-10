using FluentValidation;

namespace PushNotificationFunction.Models.Validators
{
    public class SendNotificationValidator : AbstractValidator<SendNotification>
    {
        public SendNotificationValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Message).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ApplicationId).GreaterThanOrEqualTo((byte)1);
            RuleFor(x => x.UserIds).NotNull();
        }
    }
}
