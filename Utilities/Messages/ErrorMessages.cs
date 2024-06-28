namespace NutriBest.Server.Utilities.Messages
{
    public static class ErrorMessages
    {
        public const string Exception = "Something went wrong!";
        public static class IdentityController
        {
            public const string BothPasswordsShouldMatch = "Both passwords should match!";
            public const string UserNameWithWhiteSpaces = "Username must not contain white spaces!";
            public const string ErrorResetingPassword = "Error resetting password.";
            public const string ErrorWhenFetchingRoles = "An error occured while fetching the roles!";
        }
    }
}
