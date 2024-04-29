namespace NutriBest.Server.Data
{
    public class Validation
    {
        public class Product
        {
            public const int NameMaxLength = 40;
            public const int DescriptionMaxLength = 2000;
            public const int MinPrice = 40;
            public const int MaxPrice = 40;
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
        }
    }
}
