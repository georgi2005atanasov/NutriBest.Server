using NutriBest.Server.Features.Invoices.Models;

namespace NutriBest.Server.Features.OrderDetails
{
    public interface IOrderDetailsService
    {
        Task<int> Create(int orderId, 
            string address,
            string countryName,
            string city,
            string street,
            string streetNumber,
            string postalCode,
            string paymentMethod,
            bool hasInvoice,
            InvoiceServiceModel? invoice,
            string? comment);
    }
}
