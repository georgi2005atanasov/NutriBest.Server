namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.GuestOrder;

    public class Order : Entity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        public int OrderDetailsId { get; set; }

        public bool IsFinished { get; set; }

        public bool IsConfirmed { get; set; }

        public OrderDetails? OrderDetails { get; set; }

        public int? GuestOrderId { get; set; }

        public GuestOrder? GuestOrder { get; set; }

        public int? UserOrderId { get; set; }

        public UserOrder? UserOrder { get; set; }

        [MaxLength(CommentMaxLength)]
        public string? Comment { get; set; }

        public string? SessionToken { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string? DeletedBy { get; set; }
    }
}
