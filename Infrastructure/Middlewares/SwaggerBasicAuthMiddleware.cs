﻿namespace NutriBest.Server.Infrastructure.Middlewares
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;

    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string adminEmail;
        private readonly string adminPassword;
        public SwaggerBasicAuthMiddleware(RequestDelegate next,
            string adminEmail, string adminPassword)
        {
            this.next = next;
            this.adminEmail = adminEmail;
            this.adminPassword = adminPassword;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the credentials from request header
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var inBytes = Convert.FromBase64String(header.Parameter);
                    var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];
                    // validate credentials
                    if (username.Equals(adminEmail)
                      && password.Equals(adminPassword))
                    {
                        await next.Invoke(context).ConfigureAwait(false);
                        return;
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }
    }
}
