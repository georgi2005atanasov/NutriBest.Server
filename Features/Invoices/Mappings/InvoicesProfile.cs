namespace NutriBest.Server.Features.Invoices.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Invoices.Models;

    public class InvoicesProfile : AutoMapper.Profile
    {
        public InvoicesProfile()
        {
            CreateMap<Invoice, InvoiceServiceModel>();
        }
    }
}
