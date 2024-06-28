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
    }
}
