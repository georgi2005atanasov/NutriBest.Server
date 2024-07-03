namespace NutriBest.Server.Utilities.Messages
{
    public static class ErrorMessages
    {
        public const string Exception = "Something went wrong!";
        public const string UserNotFound = "User could not be found!";

        public static class IdentityController
        {
            public const string BothPasswordsShouldMatch = "Both passwords should match!";
            public const string UserNameWithWhiteSpaces = "Username must not contain white spaces!";
            public const string ErrorResetingPassword = "Error resetting password.";
            public const string ErrorWhenFetchingRoles = "An error occured while fetching the roles!";
        }
        
        public static class AdminController
        {
            public const string UserIsAlreadyInThisRole = "'{0}' is already in the role of '{1}'!";
            public const string UserDoesNotHaveThisRole = "The user does not have this role!";
            public const string InvalidRole = "Invalid role!";
            public const string InvalidUser = "Invalid user!";
            public const string CouldNotAddRole = "Could not add the role.";
        }

        public static class EmailController
        {
            public const string EmailIsRequired = "Email is Required!";
            public const string SuccessfullySentPromoCode = "Successfully sent promo code!";
        }

        public static class BrandsController
        {
            public const string CouldNotGetBrands = "Could not get the brands!";
            public const string CouldNotCreateBrands = "Could not create brand!";
            public const string InvalidBrandName = "Invalid brand name!";
            public const string BrandAlreadyExists = "Brand with this name already exists!";
        }

        public static class FlavoursController
        {
            public const string FlavourAlreadyExists =  "Flavour with this name already exists!";
            public const string InvalidFlavour = "Invalid flavour!";
            public const string CouldNotFetchFlavours = "Could not fetch flavours!";
        }

        public static class PackagesController
        {
            public const string PackageAlreadyExists = "Package with these grams already exists!";
            public const string InvalidGrams = "Invalid Grams!";
            public const string CouldNotFetchPackages = "Could not fetch packages!";
        }

        public static class ShippingDiscountController
        {
            public const string InvalidDiscountPercentage = "The Discount Percentage must be between 1 and 100!";
            public const string PricesMustBeNumbers = "Prices must be numbers!";
            public const string InvalidDescriptionLength = "Description length must be between 5 and 50!";
            public const string ShippingDiscountDoesNotExists = "There is no shipping discount for {0}!";
            public const string CountryAlreadyHasShippingDiscount = "{0} already has a shipping discount!";
            public const string InvalidCountry = "Invalid country!";
            
        }

        public static class NutritionFactsController
        {
            public const string InvalidNutritionFacts = "Invalid nutrition facts!";
            public const string InvalidProduct = "Invalid product!";
        }
            
    }
}
