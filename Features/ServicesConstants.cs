namespace NutriBest.Server.Features
{
    public class ServicesConstants
    {
        public class PaginationConstants
        {
            public const int productsPerPage = 6;
            public const int productsPerRow = 3;

            public const int productsPerTable = 10;
            public const int productsPerRowInTable = 1;
        }

        public class Product
        {
            public const int MaxNameLength = 60;
            public const int MinNameLength = 1;
            public const int MaxDescriptionLength = 2000;
            public const int MinDescriptionLength = 5;
            public const double MinPrice = 0.1;
            public const double MaxPrice = 4000;
            public const double MinQuantity = 0;
            public const double MaxQuantity = 1000;
        }

        public class NutritionFacts
        {
            public const int MinAmount = 0;
            public const int MaxAmount = 50000;
        }

        public class Promotion
        {
            public const int MaxDescriptionLength = 50;
            public const int MinDescriptionLength = 5;
            public const int MinPercentage = 0;
            public const int MaxPercentage = 100;
            public const double MinPrice = 0.1;
            public const double MaxPrice = 3999.9;
        }
    }
}
