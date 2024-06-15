namespace NutriBest.Server.Features.Newsletter.Models
{
    public class AllSubscribersServiceModel
    {
        public List<SubscriberServiceModel> Subscribers { get; set; } = new List<SubscriberServiceModel>();

        public int TotalSubscribers { get; set; }
    }
}
