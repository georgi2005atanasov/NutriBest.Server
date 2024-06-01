namespace NutriBest.Server.Features.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using MimeKit.Text;
    using NutriBest.Server.Features.Email.Models;

    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public void SendConfirmOrderEmail(EmailConfirmOrderModel request)
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
                .container { width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
                .header { text-align: center; padding: 10px 0; }
                .header img { max-width: 100px; }
                .content { padding: 20px 0; }
                .content h1 { color: #333333; }
                .content p { color: #666666; line-height: 1.6; }
                .button-container { text-align: center; margin: 20px 0; }
                .button { background-color: #28a745; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px; }
                .footer { text-align: center; padding: 10px 0; color: #999999; font-size: 12px; }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
NutriBest
                </div>
                <div class='content'>
                    <h1>Thank you for your order!</h1>
                    <p>Hi {CustomerName},</p>
                    <p>We have received your order and it is currently being processed. Your order number is <strong>#{OrderNumber}</strong>.</p>
                    <p>Please click the button below to confirm your order:</p>
                    <div class='button-container'>
                        <a href='{ConfirmationUrl}' class='button'>Confirm Order</a>
                    </div>
                    <p>If you have any questions, feel free to contact our support team.</p>
                    <p>Thank you for shopping with us!</p>
                </div>
                <div class='footer'>
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
    }
}
