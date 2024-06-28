namespace NutriBest.Server.Features.Home
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Home.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class HomeService : IHomeService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;

        public HomeService(NutriBestDbContext db,
            IConfiguration config,
            UserManager<User> userManager)
        {
            this.db = db;
            this.config = config;
            this.userManager = userManager;
        }

        public async Task<ContactUsInfoServiceModel> ContactUsDetails()
        {
            var contactUsInfo = new ContactUsInfoServiceModel();

            var user = await userManager
                .FindByEmailAsync(config.GetSection("Admin:Email").Value);

            var profile = await db.Profiles
                .FirstAsync(x => x.UserId == user.Id);

            var address = await db.Addresses
                .FirstOrDefaultAsync(x => x.ProfileId == profile.UserId);

            contactUsInfo.PhoneNumber = user.PhoneNumber ?? "";
            contactUsInfo.Email = user.Email;
            if (address == null)
                return contactUsInfo;

            var city = await db.Cities
                .FirstAsync(x => x.Id == address.CityId);

            var country = await db.Countries
                .FirstAsync(x => x.Id == address.CountryId);

            contactUsInfo.Address = $"{address.Street} {address.StreetNumber}, {city.CityName}, {country.CountryName}";

            return contactUsInfo;
        }
    }
}
