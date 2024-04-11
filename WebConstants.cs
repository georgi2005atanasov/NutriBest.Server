namespace NutriBest.Server
{
    public static class WebConstants
    {
        public static class ProductConstants
        {
            public const int MaxNameLength = 2000;
            public const int MaxDescriptionLength = 2000;
            public const double MinPrice = 0.1;
            public const double MaxPrice = 3000;
        }

        public static class PaginationConstants
        {
            public const int productsPerPage = 1; // just for testing purposes, gotta change this
        }
    }
}
