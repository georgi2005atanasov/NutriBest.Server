namespace NutriBest.Server.Features.Email
{
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using MimeKit.Text;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.PromoCodes;
    using NutriBest.Server.Features.Email.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class EmailService : IEmailService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly IConfiguration config;
        private readonly IPromoCodeService promoCodeService;
        private readonly INotificationService notificationService;

        public EmailService(NutriBestDbContext db,
            IConfiguration config,
            IPromoCodeService promoCodeService,
            INotificationService notificationService)
        {
            this.db = db;
            this.config = config;
            this.promoCodeService = promoCodeService;
            this.notificationService = notificationService;
        }

        public async Task SendConfirmOrder(EmailConfirmOrderModel request)
        {
            var email = new MimeMessage();
            email.From
                .Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.To
                .Add(MailboxAddress
                .Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.ConfirmOrderEmail;

            // Replace placeholders with actual values
            var body = htmlTemplate
                .Replace("{CustomerName}", request.CustomerName)
                .Replace("{OrderNumber}", $"000000{request.OrderId}")
                .Replace("{ConfirmationUrl}", request.ConfirmationUrl)
                .Replace("{Date}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            await SendEmail(email);
        }

        public async Task SendNewOrderToAdmin(EmailOrderModel request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.To.Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.AdminMessageAfterCreatedOrder;

            // Replace placeholders with actual values
            var body = htmlTemplate
                .Replace("{CustomerName}", request.CustomerName)
                .Replace("{OrderNumber}", $"000000{request.OrderId}")
                .Replace("{CustomerEmail}", request.CustomerEmail)
                .Replace("{PhoneNumber}", request.PhoneNumber ?? "")
                .Replace("{OrderDate}", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                .Replace("{OrderAmount}", request.TotalPrice)
                .Replace("{OrderDetailsUrl}", request.OrderDetailsUrl)
                .Replace("{Date}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            await SendEmail(email);
        }

        public async Task SendForgottenPassword(EmailModel request, string callbackUrl)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.To.Add(MailboxAddress
                .Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.ForgottenPasswordMessage;

            var body = htmlTemplate
                .Replace("{callbackUrl}", callbackUrl)
                .Replace("{Year}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html) 
            { 
                Text = body 
            };

            await SendEmail(email);
        }

        public async Task SendPromoCode(SendPromoEmailModel request)
        {
            var email = new MimeMessage();
            email.From
                .Add(MailboxAddress
                    .Parse(config.GetSection("EmailUsername")
                    .Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.PromoCodeMessage;

            var (promoCodes, expiresIn) = await promoCodeService
                .GetByDescription(request.PromoCodeDescription);

            var code = promoCodes.FirstOrDefault();

            if (code == null)
                throw new ArgumentNullException($"There are no promo codes with description {request.PromoCodeDescription}!");

            await MarkAsSent(code);

            var body = htmlTemplate
                .Replace("{promoCode}", code)
                .Replace("{expiresIn}", $"{expiresIn}");

            email.Body = new TextPart(TextFormat.Html) 
            { 
                Text = body 
            };

            await SendEmail(email);
        }

        public async Task SendConfirmedOrderToAdmin(EmailConfirmedOrderModel request)
        {
            var email = new MimeMessage();
            email.From
                .Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.To
                .Add(MailboxAddress
                .Parse(config.GetSection("EmailUsername")
                .Value));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.ConfirmedOrderMessageToAdmin;

            var body = htmlTemplate
                .Replace("{OrderId}", request.OrderId.ToString())
                .Replace("{OrderLink}", request.OrderDetailsUrl)
                .Replace("{Year}", DateTime.UtcNow.Year.ToString());

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendJoinedToNewsletter(EmailModel request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = Messages.JoinedToNewsletterMessage;

            var body = htmlTemplate.Replace("{Year}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendMessageToSubscribers(EmailSubscribersServiceModel request, string groupType)
        {
            var subscribers = db.Newsletter
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var subscribersToSendMessage = new List<string>();

            foreach (var subscriber in subscribers)
            {
                await CheckSubscribers(subscribersToSendMessage, subscriber, groupType);
            }

            foreach (var subscriber in subscribersToSendMessage)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
                email.To.Add(MailboxAddress.Parse(subscriber));
                email.Subject = request.Subject;

                var htmlTemplate = Messages.SubscribersMessageTemplate;

                var dbSubscriber = await db.Newsletter
                    .FirstAsync(x => x.Email == subscriber);
                var newToken = Guid.NewGuid().ToString();
                dbSubscriber.VerificationToken = newToken;

                // CHANGE IN THE FUTURE
                var body = htmlTemplate
                    .Replace("{Token}", newToken)
                    .Replace("{Email}", subscriber)
                    .Replace("{Year}", $"{DateTime.UtcNow.Year}")
                    .Replace("{Body}", request.Body)
                    .Replace("{Host}", $"http://{config.GetSection("ClientHost").Value}");

                email.Body = new TextPart(TextFormat.Html) { Text = body };

                await SendEmail(email);
            }

            await db.SaveChangesAsync();
            await notificationService.SendNotificationToAdmin("success", "Successfully Sent Message to the Newsletter Subscribers!");
        }

        public async Task SendPromoCodesToSubscribers(EmailSubscribersPromoCodeServiceModel request, string groupType)
        {
            var subscribers = db.Newsletter
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var subscribersToSendMessage = new List<string>();

            foreach (var subscriber in subscribers)
            {
                await CheckSubscribers(subscribersToSendMessage, subscriber, groupType);
            }

            var promoCodesCount = await db.PromoCodes
                .Where(x => x.Description == request.PromoCodeDescription &&
                !x.IsSent &&
                x.IsValid &&
                !x.IsDeleted)
                .CountAsync();

            if (promoCodesCount < subscribersToSendMessage.Count)
            {
                throw new InvalidOperationException("The Promo Codes are not Enough!");
            }

            foreach (var subscriber in subscribersToSendMessage)
            {
                await SendPromoCode(new SendPromoEmailModel
                {
                    PromoCodeDescription = request.PromoCodeDescription,
                    Subject = request.Subject,
                    To = subscriber
                });
            }

            await notificationService.SendNotificationToAdmin("success", "Successfully Sent Promo Codes to the Newsletter Subscribers!");
        }

        private async Task SendEmail(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            int maxRetries = 5;
            int retryDelay = 5000;
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    smtp.Timeout = 120000;

                    await smtp.ConnectAsync(config.GetSection("EmailHost").Value, 
                        465, 
                        SecureSocketOptions.SslOnConnect);

                    await smtp.AuthenticateAsync(config.GetSection("EmailUsername").Value, 
                        config.GetSection("EmailPassword").Value);

                    await smtp.SendAsync(email);

                    await smtp.DisconnectAsync(true);
                    return;
                }
                catch (Exception)
                {
                    attempt++;
                    if (attempt >= maxRetries)
                    {
                        throw; // Re-throw the exception after the last attempt
                    }
                    await Task.Delay(retryDelay); // Wait before retrying
                }
                finally
                {
                    if (smtp.IsConnected)
                    {
                        await smtp.DisconnectAsync(true);
                    }
                }
            }
        }

        private async Task CheckSubscribers(List<string> subscribersToSendMessage,
            Data.Models.Newsletter subscriber,
            string groupType)
        {
            switch (groupType)
            {
                case "withOrders":
                    if (subscriber.IsAnonymous)
                    {
                        var guestOrder = await db.GuestsOrders
                            .FirstOrDefaultAsync(x => x.Email == subscriber.Email);

                        if (guestOrder != null)
                        {
                            subscribersToSendMessage.Add(subscriber.Email);
                        }
                    }
                    else
                    {
                        var userOrder = await db.UsersOrders
                           .FirstOrDefaultAsync(x => x.CreatedBy == subscriber.Email);

                        if (userOrder != null)
                        {
                            subscribersToSendMessage.Add(subscriber.Email);
                        }
                    }
                    break;
                case "withoutOrders":
                    if (subscriber.IsAnonymous)
                    {
                        var guestOrder = await db.GuestsOrders
                            .FirstOrDefaultAsync(x => x.Email == subscriber.Email);

                        if (guestOrder == null)
                        {
                            subscribersToSendMessage.Add(subscriber.Email);
                        }
                    }
                    else
                    {
                        var userOrder = await db.UsersOrders
                           .FirstOrDefaultAsync(x => x.CreatedBy == subscriber.Email);

                        if (userOrder == null)
                        {
                            subscribersToSendMessage.Add(subscriber.Email);
                        }
                    }
                    break;
                case "guests":
                    if (subscriber.IsAnonymous)
                        subscribersToSendMessage.Add(subscriber.Email);
                    break;
                case "users":
                    if (!subscriber.IsAnonymous)
                        subscribersToSendMessage.Add(subscriber.Email);
                    break;
                default:
                    subscribersToSendMessage.Add(subscriber.Email);
                    break;
            }
        }

        private async Task MarkAsSent(string code)
        {
            var codeFromDb = await db.PromoCodes
                .FirstAsync(x => x.Code == code);
            codeFromDb.IsSent = true;
            await db.SaveChangesAsync();
        }
    }
}
