namespace NutriBest.Server.Features.GuestOrders.Models
{
    using NutriBest.Server.Features.Invoices.Models;
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.GuestOrder;
    using static ServicesConstants.City;
    using static ServicesConstants.Country;

    public class GuestOrderServiceModel
    {
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        [MaxLength(CountryNameMaxLength)]
        public string CountryName { get; set; } = null!;

        [MaxLength(CityNameMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        public string Street { get; set; } = null!;

        public string? StreetNumber { get; set; }

        public int? PostalCode { get; set; }

        public bool HasInvoice { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public InvoiceServiceModel? Invoice { get; set; }

        public string? Comment { get; set; }
    }
}
