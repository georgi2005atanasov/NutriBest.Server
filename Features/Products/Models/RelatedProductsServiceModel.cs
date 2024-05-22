namespace NutriBest.Server.Features.Products.Models
{
    public class RelatedProductsServiceModel
    {
        public int ProductId { get; set; }

        public List<string>? Categories { get; set; }
    }
}
