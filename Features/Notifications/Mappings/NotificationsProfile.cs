namespace NutriBest.Server.Features.Notifications.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications.Models;

    public class NotificationsProfile : AutoMapper.Profile
    {
        public NotificationsProfile()
        {
            CreateMap<Notification, NotificationServiceModel>();
        }
    }
}
