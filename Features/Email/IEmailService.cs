﻿namespace NutriBest.Server.Features.Email
{
    using NutriBest.Server.Features.Email.Models;

    public interface IEmailService
    {
        Task SendConfirmOrder(EmailConfirmOrderModel emailModel);

        Task SendConfirmedOrderToAdmin(EmailConfirmedOrderModel emailModel);

        Task SendForgottenPassword(EmailModel request, string callbackUrl);

        Task SendNewOrderToAdmin(EmailOrderModel request);

        Task SendPromoCode(SendPromoEmailModel request);

    }
}
