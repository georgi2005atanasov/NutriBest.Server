namespace NutriBest.Server.Features.Email
{
    public static class Messages
    {
        public const string ConfirmOrderEmail = @"
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

        public const string AdminMessageAfterCreatedOrder = @"
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

        public const string ForgottenPasswordMessage = @"
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
            <p>&copy; {Year} NutriBest. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        public const string PromoCodeMessage = @"<!DOCTYPE html>
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

        public const string ConfirmedOrderMessageToAdmin = @"
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

        public const string JoinedToNewsletterMessage = @"
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

        public const string SubscribersMessageTemplate = @"
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
            <a href=""{Host}/newsletter/{Email}/{Token}"">Unsubscribe From our Newsletter.</a>
            <p>&copy; {Year} NutriBest. All rights reserved.</p>
        </div>

    </div>
</body>
</html>";
    }
}
