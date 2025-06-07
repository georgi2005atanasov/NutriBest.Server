namespace NutriBest.Server.Features.Home
{
    using NutriBest.Server.Features.Home.Models;

    public interface IHomeService
    {
        Task<ContactUsInfoServiceModel> ContactUsDetails();
    }
}
