namespace NutriBest.Server.Data
{
    public class Validation
    {
        public class Product
        {
            public const int NameMaxLength = 60;
            public const int DescriptionMaxLength = 2000;
            public const double MinPrice = 0.1;
            public const int MaxPrice = 4000;
        }

        public class Profile
        {
            public const int NameMaxLength = 40;
            public const int MaxAge = 100;
            public const int MinAge = 12;
        }

        public class NutritionFacts
        {
            public const int MinAmount = 0;
            public const int MaxAmount = 50000;
        }
    }
}
