using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Notifications.Models;

namespace NutriBest.Server.Features.Notifications.Mappings
{
    public class NotificationsProfile : AutoMapper.Profile
    {
        public NotificationsProfile()
        {
            CreateMap<Notification, NotificationServiceModel>();
        }
    }
}
