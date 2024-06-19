namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using NutriBest.Server.Infrastructure.Services;
    using System.Linq;
    using static ServicesConstants.PaginationConstants;

    public class ProfileService : IProfileService, ITransientService
    {
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUser;
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;

        public ProfileService(IIdentityService identityService,
            ICurrentUserService currentUser,
            NutriBestDbContext db,
            UserManager<User> userManager)
        {
            this.identityService = identityService;
            this.currentUser = currentUser;
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<AllProfilesServiceModel> All(int page, string? search, string? groupType)
        {
            var profiles = db.Profiles
                .OrderByDescending(x => x.CreatedOn)
                .AsQueryable();

            var allProfiles = new AllProfilesServiceModel
            {
                Profiles = new List<ProfileListingServiceModel>()
            };

            foreach (var profile in profiles)
            {
                var address = await db.Addresses
                    .FirstOrDefaultAsync(x => x.ProfileId == profile.UserId);

                City? city = new();

                if (address != null)
                {
                    city = await db.Cities
                    .FirstOrDefaultAsync(x => x.Id == address.CityId);
                }

                var user = await db.Users
                    .FirstOrDefaultAsync(x => x.Id == profile.UserId);

                var (totalOrders, userOrders) = await GetUserOrders(profile.UserId);
                decimal totalSpent = await GetTotalSpent(userOrders);

                var profileModel = new ProfileListingServiceModel
                {
                    City = city != null ? city.CityName : null,
                    Email = user!.Email,
                    MadeOn = user.CreatedOn,
                    PhoneNumber = user.PhoneNumber,
                    TotalOrders = totalOrders,
                    Name = profile.Name,
                    ProfileId = profile.UserId,
                    Roles = string.Join(", ", await userManager.GetRolesAsync(user)),
                    TotalSpent = totalSpent,
                    UserName = user.UserName,
                    IsDeleted = profile.IsDeleted
                };

                await CheckGroupType(profileModel, allProfiles.Profiles, groupType); // this also adds the entity to
                //the colleciton if it is valid
            }

            //this may be outsourced in the future, it is not so much so i do it in here
            if (search != null)
            {
                search = search.ToLower();

                allProfiles.Profiles = allProfiles
                    .Profiles
                    .Where(x => x.Email.ToLower().Contains(search) ||
                    (x.City != null && x.City.ToLower().Contains(search)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(search)) ||
                    (x.Name != null && x.Name.ToLower().Contains(search)) ||
                    x.UserName.ToLower().Contains(search) ||
                    x.Roles.ToLower().Contains(search))
                    .OrderByDescending(x => x.MadeOn)
                    .ToList();
            }

            allProfiles.TotalUsers = profiles.Count();

            allProfiles.Profiles = allProfiles.Profiles
                .Skip((page - 1) * UsersPerPage)
                .Take(UsersPerPage)
                .ToList();

            return allProfiles;
        }

        public async Task<ProfileAddressServiceModel> GetAddress()
        {
            var userId = currentUser.GetUserId();

            if (userId == null)
                throw new ArgumentNullException("Invalid user!");

            var (address, city, country) = await GetAddressWithCityAndCountry(userId);

            if (address == null)
                return new ProfileAddressServiceModel();

            var result = new ProfileAddressServiceModel
            {
                City = city != null ? city.CityName : "",
                Country = country != null ? country.CountryName : "",
                PostalCode = city != null ? city.PostalCode : null,
                Street = address.Street,
                StreetNumber = address.StreetNumber
            };

            return result;
        }

        public async Task<int> SetAddress(string street, string? streetNumber, string cityName, string countryName, int? postalCode)
        {
            var userId = currentUser.GetUserId();

            var city = await db.Cities
                .FirstOrDefaultAsync(x => x.CityName == cityName);

            var country = await db.Countries
                .FirstOrDefaultAsync(x => x.CountryName == countryName);

            if (city == null || country == null)
                throw new InvalidOperationException("Invalid city/country!");

            var address = await db.Addresses
                .FirstOrDefaultAsync(x => x.ProfileId == userId);

            if (address == null)
            {
                address = new Address();
                city.PostalCode = postalCode;
                address.Street = street;
                address.StreetNumber = streetNumber;
                address.CityId = city.Id;
                address.CountryId = country.Id;
                address.ProfileId = userId;
                db.Addresses.Add(address);
            }
            else
            {
                city.PostalCode = postalCode;
                address.Street = street;
                address.StreetNumber = streetNumber;
                address.CityId = city.Id;
                address.ProfileId = userId;
                address.CountryId = country.Id;
            }

            await db.SaveChangesAsync();

            return address.Id;
        }

        public async Task<string> UpdateProfile(string? name,
            string? userName,
            string? email,
            int? age,
            string? gender)
        {
            var id = currentUser.GetUserId();

            if (id == null)
                throw new ArgumentNullException("Invalid user!");

            var (profile, user) = await GetProfileWithUser(id);

            if (user == null || profile == null)
                return "User could not be found";

            if (age <= 0)
                return "Invalid age!";

            if (!string.IsNullOrEmpty(userName))
            {
                if (user.UserName == userName)
                    return "Username cannot be the same!";

                if (await db.Users.AnyAsync(x => x.UserName == userName))
                    return "This username is already taken!";
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (name == profile.Name)
                    return "Name cannot be the same!";

                if (await db.Users.AnyAsync(x => profile.Name == name))
                    return "User with this name already exists!";
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (email == user.Email)
                    return "Email cannot be the same!";

                if (await db.Users.AnyAsync(x => x.Email == email))
                    return $"Email '{email}' is already taken!";
            }

            if (age != null && age == profile.Age)
                return $"Age must be different than the previous";

            Gender genderRes = Gender.Unspecified;

            if (!string.IsNullOrEmpty(gender) && !Enum.TryParse<Gender>(gender, true, out genderRes))
                return $"{gender} is invalid Gender!";

            if (gender == profile.Gender.ToString())
                return "The gender must be different from the previous!";

            if (!string.IsNullOrEmpty(email))
            {
                if (await db.Newsletter.AnyAsync(x => x.Email == user.Email))
                {
                    var newsletter = await db.Newsletter
                        .FirstAsync(x => x.Email == user.Email);
                    newsletter.Email = email;
                }
                user.Email = email;
                user.NormalizedEmail = email.ToUpper();
            }


            if (!string.IsNullOrEmpty(name))
            {
                if (await db.Newsletter.AnyAsync(x => x.Name == profile.Name))
                {
                    var newsletter = await db.Newsletter
                        .FirstAsync(x => x.Name == profile.Name);
                    newsletter.Name = name;
                }
                profile.Name = name;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                user.UserName = userName;
                user.NormalizedUserName = userName.ToUpper();
            }

            if (age != null)
                profile.Age = age;

            if (!string.IsNullOrEmpty(gender))
                profile.Gender = genderRes;

            db.Users.Update(user);
            db.Profiles.Update(profile);

            await db.SaveChangesAsync();

            return "success";
        }

        public async Task<ProfileDetailsServiceModel> GetDetailsById(string id)
        {
            var (profile, user) = await GetProfileWithUser(id);

            if (user == null || profile == null)
                throw new ArgumentNullException("User could not be found");

            var (address, city, country) = await GetAddressWithCityAndCountry(id);

            var (totalOrders, userOrders) = await GetUserOrders(profile.UserId);
            decimal totalSpent = await GetTotalSpent(userOrders);

            var profileDetails = new ProfileDetailsServiceModel
            {
                MadeOn = user.CreatedOn,
                ModifiedOn = user.ModifiedOn,
                IsDeleted = profile.IsDeleted,
                City = city != null ? city.CityName : "",
                Country = country != null ? country.CountryName : "",
                Email = user.Email,
                Gender = profile.Gender.ToString(),
                Name = profile.Name,
                UserName = user.UserName!, // be aware
                Age = profile.Age,
                ProfileId = id,
                Street = address != null ? address.Street : "",
                StreetNumber = address != null ? address.StreetNumber : "",
                Roles = string.Join(", ", await userManager.GetRolesAsync(user)),
                PhoneNumber = user.PhoneNumber,
                TotalSpent = totalSpent,
                TotalOrders = totalOrders
            };

            return profileDetails;
        }

        private async Task<(int totalOrders, IQueryable<UserOrder> userOrders)> GetUserOrders(string userId)
        {
            var totalOrders = await db.UsersOrders
                    .Where(x => x.ProfileId == userId)
                    .CountAsync();

            var userOrders = db.UsersOrders
                .Where(x => x.ProfileId == userId);

            return (totalOrders, userOrders);
        }

        private async Task<(Profile? profile, User? user)> GetProfileWithUser(string id)
        {
            var user = await db.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var profile = await db.Profiles
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync();

            return (profile, user);
        }

        private async Task<(Address? address, City? city, Country? country)> GetAddressWithCityAndCountry(string userId)
        {
            var address = await db.Addresses
                .FirstOrDefaultAsync(x => x.ProfileId == userId);

            if (address == null)
                return (null, null, null);

            var city = await db.Cities
                .FirstAsync(x => x.Id == address.CityId);

            var country = await db.Countries
                .FirstAsync(x => x.Id == address.CountryId);

            return (address, city, country);
        }

        private async Task<decimal> GetTotalSpent(IQueryable<UserOrder> userOrders)
        {
            decimal totalSpent = 0;

            foreach (var userOrder in userOrders)
            {
                var order = await db.Orders
                    .FirstOrDefaultAsync(x => x.Id == userOrder.OrderId);

                if (order != null) // i do this check because i seeded made some invalid orders
                {
                    var cart = await db.Carts
                        .FirstAsync(x => x.Id == order.CartId);

                    totalSpent += cart.TotalProducts + (cart.ShippingPrice ?? 0);
                }
            }

            return totalSpent;
        }

        private async Task CheckGroupType(ProfileListingServiceModel profileModel, List<ProfileListingServiceModel> profilesToReturn, string? groupType)
        {
            switch (groupType)
            {
                case "withOrders":
                    var userOrder = await db.UsersOrders
                       .FirstOrDefaultAsync(x => x.CreatedBy == profileModel.UserName);
                    if (userOrder != null)
                    {
                        profilesToReturn.Add(profileModel);
                    }
                    break;
                case "withoutOrders":
                    userOrder = await db.UsersOrders
                       .FirstOrDefaultAsync(x => x.CreatedBy == profileModel.UserName);
                    if (userOrder == null)
                    {
                        profilesToReturn.Add(profileModel);
                    }
                    break;
                default:
                    profilesToReturn.Add(profileModel);
                    break;
            }
        }
    }
}

