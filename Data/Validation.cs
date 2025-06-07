namespace NutriBest.Server.Data
{
    public class Validation
    {
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

        public class Profile
        {
            public const int NameMaxLength = 40;
            public const int MaxAge = 100;
            public const int MinAge = 12;
        }

        public class GuestOrder
        {
            public const int NameMaxLength = 100;
            public const int EmailMaxLength = 70;
            public const int CommentMaxLength = 100;
        }

        public class Invoice
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int CompanyNameMaxLength = 50;
            public const int CompanyNameMinLength = 1;
            public const int PersonInChargeMaxLength = 100;
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

        public class Brand
        {
            public const int MaxBrandLength = 40;
            public const int MaxBrandDescriptionLength = 1000;
        }

        public class Category
        {
            public const int MaxNameLength = 40;
        }

        public class Package
        {
            public const double MinSize = 0.1;
            public const int MaxSize = 1_000_000;
            public const int MinQuantity = 0;
            public const int MaxQuantity = 2000;
        }

        public class PromoCodes
        {
            public const double MinDiscount = 0.1;
            public const double MaxDiscount = 99.9;
            public const int MaxDescriptionLength = 50;
            public const int MinDescriptionLength = 5;
        }

        public class City
        {
            public const int CityNameMaxLength = 60;
        }

        public class Country
        {
            public const int CountryNameMaxLength = 60;
        }
    }
}
