namespace NutriBest.Server.Features.Orders.Models
{
    public class UpdateOrderServiceModel
    {
        public bool IsFinished { get; set; }

        public bool IsPaid { get; set; }

        public bool IsShipped { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
