using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.OrderDetails
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Invoices.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.OrderDetailsService;

    public class OrderDetailsService : IOrderDetailsService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public OrderDetailsService(NutriBestDbContext db) 
            => this.db = db;

        public async Task<int> Create(string countryName,
            string cityName,
            string street,
            string? streetNumber,
            int? postalCode,
            string paymentMethod,
            bool hasInvoice,
            InvoiceServiceModel? invoice,
            string? comment,
            string? profileId)
        {
            var country = await db.Countries
                .FirstOrDefaultAsync(x => x.CountryName == countryName);

            if (country == null)
                throw new ArgumentNullException(WeDoNotShipToThisCountry);

            var city = await db.Cities
                .FirstOrDefaultAsync(x => x.CityName == cityName);

            if (city == null)
                throw new ArgumentNullException(WeDoNotShipToThisCity);

            if (city.CountryId != country.Id)
                throw new InvalidOperationException(InvalidCityOrCountry);

            if (!string.IsNullOrEmpty(profileId))
            {
                var address = await db.Addresses
                    .FirstOrDefaultAsync(x => x.ProfileId == profileId && !x.IsDeleted);

                if (address != null)
                    address.IsDeleted = true;

                address = new Address
                {
                    City = city,
                    CityId = city.Id,
                    Country = country,
                    CountryId = country.Id,
                    IsAnonymous = profileId == null,
                    ProfileId = profileId,
                    Street = street,
                    StreetNumber = streetNumber,
                    PostalCode = postalCode,
                };

                var orderDetails = new OrderDetails
                {
                    Address = address,
                    IsShipped = false,
                    IsPaid = false,
                    MadeOn = DateTime.UtcNow,
                    Invoice = hasInvoice && invoice != null ?
                    new Invoice
                    {
                        CompanyName = invoice.CompanyName,
                        FirstName = invoice.FirstName,
                        LastName = invoice.LastName,
                        PersonInCharge = invoice.PersonInCharge,
                        PhoneNumber = invoice.PhoneNumber,
                        VAT = invoice.VAT,
                        Bullstat = invoice.Bullstat
                    } : null,
                    PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod)
                };

                db.OrdersDetails.Add(orderDetails);

                await db.SaveChangesAsync();

                return orderDetails.Id;
            }
            else
            {
                var address = new Address
                {
                    City = city,
                    CityId = city.Id,
                    Country = country,
                    CountryId = country.Id,
                    IsAnonymous = true,
                    Street = street,
                    StreetNumber = streetNumber,
                    PostalCode = postalCode,
                };

                var orderDetails = new OrderDetails
                {
                    Address = address,
                    IsShipped = false,
                    IsPaid = false,
                    MadeOn = DateTime.UtcNow,
                    Invoice = hasInvoice && invoice != null ?
                    new Invoice
                    {
                        CompanyName = invoice.CompanyName,
                        FirstName = invoice.FirstName,
                        LastName = invoice.LastName,
                        PersonInCharge = invoice.PersonInCharge,
                        PhoneNumber = invoice.PhoneNumber,
                        VAT = invoice.VAT,
                        Bullstat = invoice.Bullstat
                    } : null,
                    PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod)
                };

                db.OrdersDetails.Add(orderDetails);

                await db.SaveChangesAsync();

                return orderDetails.Id;
            }
        }

    }
}
