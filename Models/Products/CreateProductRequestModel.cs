namespace NutriBest.Server.Models.Products
{
    public class CreateProductRequestModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public IFormFile Image { get; set; } = null!;
    }
}
