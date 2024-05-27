namespace NutriBest.Server.Features.OrderDetails
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Invoices.Models;

    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly NutriBestDbContext db;

        public OrderDetailsService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<int> CreateAnonymous(string countryName,
            string cityName,
            string street,
            int? streetNumber,
            int? postalCode,
            string paymentMethod,
            bool hasInvoice,
            InvoiceServiceModel? invoice,
            string? comment)
        {
            var country = await db.Countries
                .FirstOrDefaultAsync(x => x.CountryName == countryName);

            if (country == null)
                throw new ArgumentNullException("We do not ship to this country!");

            var city = await db.Cities
                .FirstOrDefaultAsync(x => x.CityName == cityName);

            if (city == null)
                throw new ArgumentNullException("We do not ship to this city!");

            if (city.CountryId != country.Id)
                throw new InvalidOperationException("Invalid country/city!");

            var address = new Address
            {
                City = city,
                CityId = city.Id,
                Country = country,
                CountryId = country.Id,
                IsAnonymous = true,
                Street = street,
                StreetNumber = streetNumber
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
