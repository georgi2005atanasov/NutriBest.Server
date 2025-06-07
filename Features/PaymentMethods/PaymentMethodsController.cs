namespace NutriBest.Server.Features.PaymentMethods
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data.Enums;

    public class PaymentMethodsController : ApiController
    {
        private readonly IMemoryCache memoryCache;

        public PaymentMethodsController(IMemoryCache memoryCache)
            => this.memoryCache = memoryCache;

        [HttpGet]
        public ActionResult<string[]> GetPaymentMethods()
        {
            try
            {
                const string cacheKey = "allPaymentMethods";
                if (!memoryCache.TryGetValue(cacheKey, out string[] paymentMethods))
                {
                    paymentMethods = Enum.GetNames(typeof(PaymentMethod));

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    };

                    memoryCache.Set(cacheKey, paymentMethods, cacheEntryOptions);
                }

                return Ok(paymentMethods);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
