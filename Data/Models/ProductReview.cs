namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductReview
    {
        [Key]
        [Required]
        public int ReviewId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        public string? ProfileId { get; set; }

        public Profile? Profile { get; set; }

        [Required]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
