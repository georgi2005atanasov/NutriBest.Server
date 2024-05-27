using NutriBest.Server.Features.Invoices.Models;

namespace NutriBest.Server.Features.OrderDetails
{
    public interface IOrderDetailsService
    {
        Task<int> CreateAnonymous(//orderId
            string countryName,
            string city,
            string street,
            int? streetNumber,
            int? postalCode,
            string paymentMethod,
            bool hasInvoice,
            InvoiceServiceModel? invoice,
            string? comment);
    }
}
