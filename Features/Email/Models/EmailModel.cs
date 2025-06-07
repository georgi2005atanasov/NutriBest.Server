namespace NutriBest.Server.Features.Email.Models
{
    public class EmailModel
    {
        public string To { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (EmailModel)obj;
            return Body == other.Body &&
                   To == other.To &&
                   Subject == other.Subject;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Body, Subject, To);
        }
    }
}
