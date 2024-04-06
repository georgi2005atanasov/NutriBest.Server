namespace NutriBest.Server.Data.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        //public IFormFile Image { get; set; } = null!;

    }
}
