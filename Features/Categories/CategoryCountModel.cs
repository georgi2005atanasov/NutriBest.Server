using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Features.Categories
{
    public class CategoryCountModel
    {
        public string Category { get; set; } = null!;
        public int Count { get; set; }
    }
}
