namespace NutriBest.Server.Features.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using MimeKit.Text;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Email.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.PromoCodes;
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
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = @"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Order Confirmation</title>
            <style>
                body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
                #container { width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
                #header { text-align: center; padding: 10px 0; }
                #header img { max-width: 100px; }
                #content { padding: 20px 0; }
                #content h1 { color: #333333; }
                #content p { color: #666666; line-height: 1.6; }
                #button-container { text-align: center; margin: 20px 0; }
                #button { background-color: #28a745; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px; }
                #footer { text-align: center; padding: 10px 0; color: #999999; font-size: 12px; }
            </style>
        </head>
        <body>
            <div id='container'>
                <div id='header'>
NutriBest
                </div>
                <div id='content'>
                    <h1>Thank you for your order!</h1>
                    <p>Hi {CustomerName},</p>
                    <p>We have received your order and it is currently being processed. Your order number is <strong>#{OrderNumber}</strong>.</p>
                    <p>Please click the button below to confirm your order:</p>
                    <div class='button-container'>
                        <a href='{ConfirmationUrl}' id='button'>Confirm Order</a>
                    </div>
                    <p>If you have any questions, feel free to contact our support team.</p>
                    <p>Thank you for shopping with us!</p>
                </div>
                <div id='footer'>
                    <p>&copy; {Date} NutriBest. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

            // Replace placeholders with actual values
            var body = htmlTemplate
                .Replace("{CustomerName}", request.CustomerName)
                .Replace("{OrderNumber}", $"000000{request.OrderId}")
                .Replace("{ConfirmationUrl}", request.ConfirmationUrl)
                .Replace("{Date}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendNewOrderToAdmin(EmailOrderModel request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.Subject = request.Subject;

            var htmlTemplate = @"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>New Order Notification</title>
        <style>
            body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
            #container { width: 100%; max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
            #header { text-align: center; padding: 10px 0; }
            #header img { max-width: 100px; }
            #content { padding: 20px 0; }
            #content h2 { color: #333333; }
            #content p { color: #666666; line-height: 1.6; }
            #order-details { margin: 20px 0; }
            #order-details table { width: 100%; border-collapse: collapse; }
            #order-details th, #order-details td { border: 1px solid #dddddd; text-align: left; padding: 8px; }
            #order-details th { background-color: #f2f2f2; }
            #footer { text-align: center; padding: 10px 0; color: #999999; font-size: 12px; }
        </style>
    </head>
    <body>
        <div id='container'>
            <div id='content'>
                <h2>New Order Received!</h2>
                <p><strong>{CustomerName}</strong> has just placed an order.</p>
                <p>Order Details:</p>
                <div id='order-details'>
                    <table>
                        <tr>
                            <th>Order Number: </th>
                            <td>#{OrderNumber}</td>
                        </tr>
                        <tr>
                            <th>Customer Name: </th>
                            <td>{CustomerName}</td>
                        </tr>
                        <tr>
                            <th>Email: </th>
                            <td>{CustomerEmail}</td>
                        </tr>
                        <tr>
                            <th>Phone Number: </th>
                            <td>{PhoneNumber}</td>
                        </tr>
                        <tr>
                            <th>Order Date: </th>
                            <td>{OrderDate}</td>
                        </tr>
                        <tr>
                            <th>Total Amount: </th>
                            <td>{OrderAmount}BGN</td>
                        </tr>
                    </table>
                </div>
                <p>Click <a href='{OrderDetailsUrl}'>here</a> to view the order details in the admin panel.</p>
                <p>Thank you!</p>
            </div>
            <div id='footer'>
                <p>&copy; {Date} NutriBest. All rights reserved.</p>
            </div>
        </div>
    </body>
    </html>";

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

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendForgottenPassword(EmailModel request, string callbackUrl)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        .header {
            text-align: center;
            padding: 10px 0;
            border-bottom: 1px solid #e5e5e5;
        }
        .header h1 {
            margin: 0;
            color: #333;
        }
        .content {
            padding: 20px;
            text-align: center;
        }
        .content p {
            font-size: 16px;
            line-height: 1.5;
        }
        .content a {
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #007BFF;
            color: #ffffff;
            text-decoration: none;
            border-radius: 5px;
        }
        .content a:hover {
            background-color: #0056b3;
        }
        .footer {
            text-align: center;
            padding: 20px 0;
            font-size: 12px;
            color: #777;
            border-top: 1px solid #e5e5e5;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <p>Hi there,</p>
            <p>We received a request to reset your password. Click the button below to reset it.</p>
            <a href='{callbackUrl}'>Reset Password</a>
            <p>If you did not request a password reset, please ignore this email.</p>
            <p>Thank you,<br>Your Company Name</p>
        </div>
        <div class='footer'>
            <p>&copy; {Year} Your Company Name. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            var body = htmlTemplate
                .Replace("{callbackUrl}", callbackUrl)
                .Replace("{Year}", $"{DateTime.UtcNow.Year}");

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendPromoCode(SendPromoEmailModel request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            var htmlTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>NutriBest Promo</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }

        .email-container {
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        .header {
            background-color: #4caf50;
            padding: 20px;
            text-align: center;
        }

        .logo {
            max-width: 150px;
        }

        .content {
            padding: 20px;
        }

        .content h1 {
            color: #4caf50;
        }

        .content p {
            color: #333333;
            line-height: 1.6;
        }

        .button {
            display: inline-block;
            padding: 10px 20px;
            background-color: #4caf50;
            color: #ffffff;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
        }

        .button:hover {
            background-color: #45a049;
        }

        .footer {
            background-color: #f4f4f4;
            padding: 20px;
            text-align: center;
        }

        .social-icons img {
            width: 30px;
            margin: 0 10px;
        }

        .social-icons a {
            text-decoration: none;
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
        </div>
        <div class=""content"">
            <h1>Welcome to NutriBest!</h1>
            <p>Dear valued customer,</p>
            <p>We are excited to offer you an exclusive promocode as a token of our appreciation for your loyalty.</p>
            <p>Use the code <strong>{promoCode}</strong> at checkout to get surprising discount from your next purchase!</p>
            <p>But you have to hurry up, because after <strong>{expiresIn}</strong> days the promocode won't be valid!</p>
            <p>Visit our website to explore our wide range of fitness supplements designed to help you achieve your health goals.</p>
            <a href=""http://localhost:5173/"" class=""button"">Shop Now</a>
        </div>
        <div class=""footer"">
            <p>Thank you for choosing NutriBest!</p>
            <p>Follow us on:</p>
            <div class=""social-icons"">
                <a href=""https://www.facebook.com/georgi.atanasov.5891004/?locale=bg_BG""><img src=""https://www.facebook.com/"" alt=""Facebook""></a>
                <a href=""https://www.instagram.com/atanasowww7/""><img src=""https://www.instagram.com/"" alt=""Instagram""></a>
            </div>
        </div>
    </div>
</body>
</html>";
            var (promoCodes, expiresIn) = await promoCodeService.GetByDescription(request.PromoCodeDescription);

            var code = promoCodes.FirstOrDefault();
            await MarkAsSent(code);

            if (code == null)
                throw new ArgumentNullException($"There are no promo codes with description {request.PromoCodeDescription}!");

            var body = htmlTemplate
                .Replace("{promoCode}", code)
                .Replace("{expiresIn}", $"{expiresIn}");

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await SendEmail(email);
        }

        public async Task SendConfirmedOrderToAdmin(EmailConfirmedOrderModel request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
            email.Subject = request.Subject;

            var htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Order Confirmation</title>
    <style>
        body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
        #container { width: 100%; max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
        #header { text-align: center; padding: 10px 0; }
        #content { padding: 20px 0; }
        #content h2 { color: #333333; }
        #content p { color: #666666; line-height: 1.6; }
        #order-details { margin: 20px 0; }
        #footer { text-align: center; padding: 10px 0; color: #999999; font-size: 12px; }
        .button {
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #007BFF;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <div id=""container"">
        <div id=""header"">
            <h2>Order Confirmed!</h2>
        </div>
        <div id=""content"">
            <p>An order with ID <strong>#{OrderId}</strong> has been confirmed.</p>
            <p>Click the button below to view the order details:</p>
            <a href=""{OrderLink}"" class=""button"">View Order</a>
        </div>
        <div id=""footer"">
            <p>&copy; {Year} NutriBest. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

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

            var htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Newsletter Subscription</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333333;
            margin: 0;
            padding: 0;
        }
        .container {
            width: 80%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        .header {
            text-align: center;
            padding-bottom: 20px;
        }
        .header img {
            width: 100px;
        }
        .content {
            text-align: center;
        }
        .content h1 {
            color: #007BFF;
        }
        .footer {
            text-align: center;
            padding-top: 20px;
            font-size: 12px;
            color: #999999;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""content"">
            <h1>Subscription Successful!</h1>
            <p>Thank you for subscribing to our newsletter.</p>
            <p>You will now receive the latest updates and news.</p>
        </div>
        <div class=""footer"">
            <p>If you did not subscribe to this newsletter, please ignore this email.</p>
            <p>&copy; {Year} NutriBest. All rights reserved.</p>
        </div>

    </div>
</body>
</html>";

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

                var htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Newsletter Subscription</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333333;
            margin: 0;
            padding: 0;
        }
        .container {
            width: 80%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        .content {
            text-align: center;
        }
        .footer {
            text-align: center;
            padding-top: 20px;
            font-size: 12px;
            color: #999999;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""content"">
            <p>{Body}</p>
        </div>
        <div class=""footer"">
            <a href=""{Host}/newsletter/{Email}"">Unsubscribe From our Newsletter.</a>
            <p>&copy; {Year} NutriBest. All rights reserved.</p>
        </div>

    </div>
</body>
</html>";

                var body = htmlTemplate
                    .Replace("{Email}", subscriber)
                    .Replace("{Year}", $"{DateTime.UtcNow.Year}")
                    .Replace("{Body}", request.Body)
                    .Replace("{Host}", $"http://{config.GetSection("ClientHost").Value}");

                email.Body = new TextPart(TextFormat.Html) { Text = body };

                await SendEmail(email);
            }
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
                    await smtp.ConnectAsync(config.GetSection("EmailHost").Value, 465, SecureSocketOptions.SslOnConnect);
                    await smtp.AuthenticateAsync(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
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

        private async Task MarkAsSent(string? code)
        {
            var codeFromDb = await db.PromoCodes
                .FirstAsync(x => x.Code == code);
            codeFromDb.IsSent = true;
            await db.SaveChangesAsync();
        }
    }
}
