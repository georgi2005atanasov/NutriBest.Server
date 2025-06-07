namespace NutriBest.Server.Features.Invoices
{
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class InvoiceService : IInvoiceService, ITransientService
    {
        public Task<int> Create()
        {
            throw new NotImplementedException();
        }
    }
}
