namespace NutriBest.Server.Features.GuestsOrders.Models
{
    using System.ComponentModel.DataAnnotations;
    using NutriBest.Server.Features.Invoices.Models;
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

        [Required]
        [MaxLength(CountryNameMaxLength)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(CityNameMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        public string Street { get; set; } = null!;

        public string? StreetNumber { get; set; }

        public string? PostalCode { get; set; }

        public bool HasInvoice { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = null!;

        public InvoiceServiceModel? Invoice { get; set; }

        [MaxLength(CommentMaxLength)]
        public string? Comment { get; set; }
    }
}
