namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.GuestOrder;

    public class Order
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

        public GuestOrder? GuestOrder { get; set; }

        public UserOrder? UserOrder { get; set; }

        [MaxLength(CommentMaxLength)]
        public string? Comment { get; set; }
    }
}
