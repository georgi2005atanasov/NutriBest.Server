namespace NutriBest.Server.Features.Newsletter.Models
{
    public class AllSubscribersServiceModel
    {
        public int TotalSubscribers { get; set; }

        public List<SubscriberServiceModel> Subscribers { get; set; } = new List<SubscriberServiceModel>();
    }
}
