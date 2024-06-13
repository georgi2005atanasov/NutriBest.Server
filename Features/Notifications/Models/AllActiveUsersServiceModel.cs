namespace NutriBest.Server.Features.Notifications.Models
{
    public class AllActiveUsersServiceModel
    {
        public List<ActiveUserServiceModel> ActiveUsers { get; set; } = new List<ActiveUserServiceModel>();
    }
}
