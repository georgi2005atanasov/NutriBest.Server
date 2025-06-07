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

        public static class ProfileController
        {
            public const string UserCouldNotBeFound = "User could not be found!";
            public const string InvalidUser = "Invalid user!";
            public const string InvalidCityOrCountry = "Invalid city/country!";
            public const string InvalidAge = "Invalid age!";
            public const string UserNameAlreadyTaken = "This username is already taken!";
            public const string UserNameCannotBeTheSame = "Username cannot be the same!";
            public const string NameCannotBeTheSame = "Name cannot be the same!";
            public const string EmailCannotBeTheSame = "Email cannot be the same!";
            public const string EmailAlreadyTaken = "Email '{0}' is already taken!";
            public const string AgeCannotBeTheSame = "Age must be different than the previous";
            public const string GenderCannotBeTheSame = "The gender must be different from the previous!";
            public const string InvalidGender = "{0} is invalid Gender!";
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
            public const string InvalidPackage = "Invalid package!";
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
         
        public static class CategoriesController
        {
            public const string CouldNotFetchCategories = "Could not fetch categories!";
            public const string CategoryAlreadyExists = "Category with this name already exists!";
            public const string InvalidCategory = "Invalid category!";
        }

        public static class ProductsController
        {
            public const string ProductsSpecificationAreRequired = "You must add some product specifications!";
            public const string ImageIsRequired = "Image is required!";
            public const string ProductAlreadyExists = "Product with this name already exists!";
            public const string PricesMustBeNumbers = "Prices must be numbers!";
            public const string InvalidPriceRange = "Price must be bigger than zero and less than 4000!";
            public const string AtLeastOneCategory = "You have to choose at least 1 category!";
            public const string MustHaveBiggerPriceBecauseOfTheAppliedPromotion = "The price must be bigger, because of the applied promotion!";
            public const string InvalidProduct = "Invalid product!";
            public const string ProductDoesNotHaveThisPromotion = "The product does not have this promotion!";
            public const string InvalidPriceFormat = "Invalid price format: {0}";
        }

        public static class PromoCodeController
        {
            public const string EnterValidPercentage = "Discount percentage must be betweeen 0% and 100%!";
            public const string InvalidPromoCode = "Invalid Promo Code!";
        }

        public static class PromotionsController
        {
            public const string PromotionIsNotActive = "Promotion is not active!";
            public const string InvalidDiscount = "The discount cannot be more than 99.9%!";
            public const string DiscountIsRequired = "You have to make some kind of discount!";
            public const string InvalidPromotion = "The promotion is not valid!";
            public const string PromotionDoesNotExists = "Promotion does not exist!";
            public const string NewDiscountCannotBeApplied = "The discount cannot be applied to all the products!";
            public const string PromotionAlreadyExists = "Promotion with this description already exists!";
            public const string TypeOfDiscountIsRequired = "You have to choose one type of discount!";
            public const string StartDateMustBeBeforeEndDate = "The start date must be before the end date!";
            public const string LeastPromotionDurationRequired = "The promotion must be with at least one day duration!";
            public const string CannotChangePromotionStatus = "You cannot change the status of the promotion!";
        }

        public static class NewsletterController
        {
            public const string UserIsAlreadySubscribed = "'{0}' is already subscribed!";
        }

        public static class CartController
        {
            public const string InvalidProductCount = "Invalid product count!";
            public const string ProductDoesNotExists = "This product does not exist!";
            public const string ProductNotFound = "Product not found in the cart.";
            public const string ProductsAreNotAvailableWithThisCount = "Sorry, we have {0} units of this product available.";
            public const string CartIsAlreadyEmpty = "The cart is already empty!";
            public const string YouHaveToAddProducts = "You have to add products to the cart!";
        }

        public static class OrdersController
        {
            public const string OrderCouldNotBeDeleted = "Order Could not be Deleted!";
            public const string OrderMustBeFinishedBeforeDeletingIt = "The order must be finished before deleting it!";
        }

        public static class UsersOrdersController
        {
            public const string FillInvoiceForm = "Fill the invoice form!";
            public const string InvalidPostalCode = "Invalid postal code!";
            public const string InvalidPaymentMethod = "Invalid payment method!";
            public const string PurchaseIsRequiredToHaveSomething = "You have to purchase something!";
            public const string InvalidUser = "Invalid user!";
        }

        public static class GuestsOrdersController
        {
            public const string FillInvoiceForm = "Fill the invoice form!";
            public const string InvalidPostalCode = "Invalid postal code!";
            public const string InvalidPaymentMethod = "Invalid payment method!";
            public const string PurchaseIsRequiredToHaveSomething = "You have to purchase something!";
            public const string InvalidUser = "Invalid user!";
            public const string UserWithThisEmailAlreadyExists = "User with this email already exists!";
        }

        public static class OrderDetailsService
        {
            public const string WeDoNotShipToThisCountry = "We do not ship to this country!";
            public const string WeDoNotShipToThisCity = "We do not ship to this city!";
            public const string InvalidCityOrCountry = "Invalid city/country!";
        }

        public static class NotificationService
        {
            public const string LowInStock = "Low in Stock!";
            public const string StockIsRunningLow = "Stock is running low!";
            public const string OutOfStock = "Out of Stock!";
            public const string BeAwareOfTheProductQuantity = "Be Aware That Product With Name '{0}' has Quantity of {1}.";
            public const string CriticallyLowStockLevels = "'{0}' stock levels are critically low! ({1} left)";
            public const string OutOfStockMesage = "'{0}' is Out of Stock! ({1})";
            public const string CannotFulfillOrder = "Cannot Fulfill Order {0}. Product '{1}' is Out of Stock!";
        }
    }
}
