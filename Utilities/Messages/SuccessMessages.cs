namespace NutriBest.Server.Utilities.Messages
{
    public static class SuccessMessages
    {
        public static class IdentityController
        {
            public const string SuccessfullyAddedUser = "Successfully added new user!";
            public const string PasswordResetSuccessful = "Password reset successful.";
            public const string UserHasJustRegistered = "'{0}' Has Just Registered!";
            public const string UserHasJustLoggedIn= "'{0}' Has Just Logged In!";
            
        }

        public static class AdminController
        {
            public const string SuccessfullyAddedRole = "Successfully added role '{0}' to '{1}'!";
            public const string SuccessfullyRemovedRole = "Successfully removed role '{0}' from '{1}'!";
            public const string UserHasJustLoggedIn = "'{0}' Has Just Logged In!";
            public const string SuccessfullyRestoredProfile = "Successfully restored profile with email '{0}'!";
        }

        public static class EmailController
        {
            public const string IfEmailIsValidLinkIsSent = "If the email is valid, a password reset link has been sent.";
        }

        public static class NotificationService
        {
            public const string UserSignedUpForNewsletter = "'{0}' Has Just Signed up For the Newsletter";
            public const string UserHasJustMadeAnOrder = "'{0}' Has Just Made an Order for {1}BGN!";
            public const string OrderHasJustBeenConfirmed = "Order #000000{0} Has Just Been Confirmed!";
        }
    }
}
