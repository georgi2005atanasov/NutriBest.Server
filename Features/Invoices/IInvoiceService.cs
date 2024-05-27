namespace NutriBest.Server.Features.Invoices
{
    public interface IInvoiceService
    {
        Task<int> Create();
    }
}
