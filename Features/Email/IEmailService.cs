namespace NutriBest.Server.Features.Email
{
    using NutriBest.Server.Features.Email.Models;

    public interface IEmailService
    {
        void SendConfirmOrder(EmailConfirmOrderModel emailModel);

        void SendForgottenPassword(EmailModel request, string callbackUrl);
    }
}
