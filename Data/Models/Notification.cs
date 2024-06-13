using NutriBest.Server.Data.Enums;

namespace NutriBest.Server.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public Priority Priority { get; set; }

        public int? ProductId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
