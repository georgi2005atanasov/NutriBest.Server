namespace NutriBest.Server.Features.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using MimeKit.Text;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Email.Models;
    using NutriBest.Server.Features.PromoCodes;

    public class EmailService : IEmailService
    {
        private readonly NutriBestDbContext db;
        private readonly IConfiguration config;
        private readonly IPromoCodeService promoCodeService;

        public EmailService(NutriBestDbContext db,
            IConfiguration config,
            IPromoCodeService promoCodeService)
        {
            this.db = db;
            this.config = config;
            this.promoCodeService = promoCodeService;
        }

        public void SendConfirmOrder(EmailConfirmOrderModel request)
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

            using var smtp = new SmtpClient();
            smtp.Connect(config.GetSection("EmailHost").Value, 465, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendForgottenPassword(EmailModel request, string callbackUrl)
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
            <p>&copy; 2024 Your Company Name. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            var body = htmlTemplate.Replace("{callbackUrl}", callbackUrl);

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(config.GetSection("EmailHost").Value, 465, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task SendPromoCode(SendEmailModel request)
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
            <img src=""https://yourwebsite.com/logo.png"" alt=""NutriBest Logo"" class=""logo"">
        </div>
        <div class=""content"">
            <h1>Welcome to NutriBest!</h1>
            <p>Dear valued customer,</p>
            <p>We are excited to offer you an exclusive promocode as a token of our appreciation for your loyalty.</p>
            <p>Use the code <strong>{promoCode}</strong> at checkout to get 20% off your next purchase!</p>
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
                .Replace("{promoCode}", code);

            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using var smtp = new SmtpClient();
            smtp.Connect(config.GetSection("EmailHost").Value, 465, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
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
