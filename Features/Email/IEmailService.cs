namespace NutriBest.Server.Features.Email
{
    using NutriBest.Server.Features.Email.Models;

    public interface IEmailService
    {
        void SendConfirmOrderEmail(EmailConfirmOrderModel emailModel);
    }
}
