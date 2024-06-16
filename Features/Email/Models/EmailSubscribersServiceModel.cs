namespace NutriBest.Server.Features.Email.Models
{
    public class EmailSubscribersServiceModel
    {
        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty!;
    }
}
