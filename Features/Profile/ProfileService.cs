namespace NutriBest.Server.Features.Admin
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Services;
    using static ServicesConstants.PaginationConstants;

    public class ProfileService : IProfileService
    {
        private readonly ICurrentUserService currentUser;
        private readonly NutriBestDbContext db;

        public ProfileService(ICurrentUserService currentUser, NutriBestDbContext db)
        {
            this.currentUser = currentUser;
            this.db = db;
        }

        public async Task<AllProfilesServiceModel> All(int page, string? search)
        {
            var profiles = db.Profiles
                .OrderByDescending(x => x.CreatedOn)
                .AsQueryable();

            var allProfiles = new AllProfilesServiceModel
            {
                AllUsers = profiles.Count(),
                Profiles = new List<ProfileListingServiceModel>()
            };

            profiles = profiles
                .Skip((page - 1) * UsersPerPage)
                .Take(UsersPerPage)
                .AsQueryable();

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

                var totalOrders = await db.UsersOrders
                    .Where(x => x.ProfileId == profile.UserId)
                    .CountAsync();

                var profileModel = new ProfileListingServiceModel
                {
                    City = city != null ? city.CityName : null,
                    Email = user!.Email,
                    MadeOn = user.CreatedOn,
                    PhoneNumber = user.PhoneNumber,
                    TotalOrders = totalOrders,
                    Name = profile.Name,
                };

                allProfiles.Profiles.Add(profileModel);
            }

            if (search != null)
            {
                search = search.ToLower();

                allProfiles.Profiles = allProfiles
                    .Profiles
                    .Where(x => x.Email.ToLower().Contains(search) ||
                    x.City.ToLower().Contains(search) ||
                    x.PhoneNumber.ToLower().Contains(search))
                    .ToList();
            }

            return allProfiles;
        }

        public async Task<ProfileAddressServiceModel> GetAddress()
        {
            var userId = currentUser.GetUserId();

            var address = await db.Addresses
                .FirstOrDefaultAsync(x => x.ProfileId == userId);

            if (address == null)
                return new ProfileAddressServiceModel();

            var city = await db.Cities
                .FirstAsync(x => x.Id == address.CityId);
            var country = await db.Countries
                .FirstAsync(x => x.Id == address.CountryId);

            var result = new ProfileAddressServiceModel
            {
                City = city.CityName,
                Country = country.CountryName,
                PostalCode = city.PostalCode,
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

            var user = await db.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var profile = await db.Profiles
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync();

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
                user.Email = email;

            if (!string.IsNullOrEmpty(name))
                profile.Name = name;

            if (!string.IsNullOrEmpty(userName))
                user.UserName = userName;

            if (age != null)
                profile.Age = age;

            if (!string.IsNullOrEmpty(gender))
                profile.Gender = genderRes;

            db.Users.Update(user);
            db.Profiles.Update(profile);

            await db.SaveChangesAsync();

            return "success";
        }
    }
}
