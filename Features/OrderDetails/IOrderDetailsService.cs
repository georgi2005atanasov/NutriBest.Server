namespace NutriBest.Server.Features.OrderDetails
{
    using NutriBest.Server.Features.Invoices.Models;

    public interface IOrderDetailsService
    {
        Task<int> Create(string countryName,
            string city,
            string street,
            string? streetNumber,
            int? postalCode,
            string paymentMethod,
            bool hasInvoice,
            InvoiceServiceModel? invoice,
            string? comment,
            string profileId = "");
    }
}
