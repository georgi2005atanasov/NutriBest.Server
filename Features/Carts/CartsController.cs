using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Carts
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.CartController;
    using static ErrorMessages.PromoCodeController;

    public class CartsController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;

        public CartsController(NutriBestDbContext db)
            => this.db = db;

        [HttpGet]
        [AllowAnonymous]
        [Route("/Cart/Get")]
        public async Task<IActionResult> GetCartFromSession()
        {
            try
            {
                var cart = GetSessionCart() ?? new CartServiceModel();

                foreach (var cartProduct in cart.CartProducts)
                {
                    var product = await db.Products
                        .Select(x => new ProductListingServiceModel
                        {
                            ProductId = x.ProductId,
                            Name = x.Name,
                            Price = x.StartingPrice,
                            Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                            Quantity = x.Quantity,
                            PromotionId = x.PromotionId,
                            Brand = x.Brand!.Name // be aware
                        })
                        .FirstAsync(x => x.ProductId == cartProduct.ProductId);

                    if (product.PromotionId != null)
                    {
                        var promotion = await db.Promotions
                            .FirstAsync(x => x.PromotionId == product.PromotionId);

                        if (promotion.DiscountPercentage != null)
                        {
                            product.DiscountPercentage = promotion.DiscountPercentage;
                        }

                        if (promotion.DiscountAmount != null)
                        {
                            //productFromDb.Price * ((100 - (decimal)promotion.DiscountAmount) / 100) * cartProduct.Count
                            product.DiscountPercentage = (promotion.DiscountAmount * 100) / product.Price;
                        }
                    }

                    cartProduct.Product = product;
                }

                return Ok(cart);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Cart/Set")]
        public async Task<ActionResult> SetProductCount([FromBody] CartProductServiceModel cartProduct)
        {
            if (cartProduct.Count <= 0)
                return BadRequest(new FailResponse
                {
                    Message = InvalidProductCount
                });

            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                await DisablePromoCode(cart, cart.Code ?? "");

                if (cart.CartProducts == null)
                    cart.CartProducts = new List<CartProductServiceModel>();

                var existingProduct = GetExistingProduct(cart, cartProduct);
                var productPackageFlavourFromDb = await GetProductPackageFlavourFromDb(cartProduct);

                if (productPackageFlavourFromDb == null)
                    return BadRequest(new FailResponse
                    {
                        Message = ProductDoesNotExists
                    });

                var productFromDb = await db.Products
                    .FirstAsync(x => x.ProductId == productPackageFlavourFromDb.ProductId);

                if (existingProduct != null)
                {
                    await CalculateTotalAmounts(isSubtracting: true,
                        cart,
                        existingProduct,
                        productFromDb);

                    existingProduct.Count = cartProduct.Count;

                    if (!CanRemoveProduct(productPackageFlavourFromDb.Quantity, existingProduct.Count))
                        return BadRequest(new FailResponse
                        {
                            Message = $"Sorry, we have {productPackageFlavourFromDb.Quantity} units of this product available."
                        });
                }
                else
                {
                    cart.CartProducts.Add(cartProduct);

                    if (!CanRemoveProduct(productPackageFlavourFromDb.Quantity, cartProduct.Count))
                        return BadRequest(new FailResponse
                        {

                            Message = string.Format(ProductsAreNotAvailableWithThisCount, productPackageFlavourFromDb.Quantity)
                        });
                    existingProduct = cartProduct;
                }

                await CalculateTotalAmounts(isSubtracting: false,
                    cart,
                    existingProduct!, // be aware
                    productFromDb);

                await EnablePromoCode(cart);

                await SetSessionCart(cart);
                return Ok(cart);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Cart/Add")]
        public async Task<ActionResult<string>> AddToCart([FromBody] CartProductServiceModel cartProduct)
        {
            if (cartProduct.Count <= 0)
                return BadRequest(new FailResponse
                {
                    Message = InvalidProductCount
                });

            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.CartProducts == null)
                    cart.CartProducts = new List<CartProductServiceModel>();

                await DisablePromoCode(cart, cart.Code ?? "");

                var existingProduct = GetExistingProduct(cart, cartProduct);

                var productPackageFlavourFromDb = await GetProductPackageFlavourFromDb(cartProduct);

                if (productPackageFlavourFromDb == null)
                    return BadRequest(new FailResponse
                    {
                        Message = ProductDoesNotExists
                    });

                var productFromDb = await db.Products
                    .FirstAsync(x => x.ProductId == productPackageFlavourFromDb.ProductId);

                if (existingProduct != null)
                {
                    existingProduct.Count += cartProduct.Count;

                    if (!CanRemoveProduct(productPackageFlavourFromDb.Quantity, existingProduct.Count))
                        return BadRequest(new FailResponse
                        {
                            Message = string.Format(ProductsAreNotAvailableWithThisCount, productPackageFlavourFromDb.Quantity)
                        });
                }
                else
                {
                    cart.CartProducts.Add(cartProduct);

                    if (!CanRemoveProduct(productPackageFlavourFromDb.Quantity, cartProduct.Count))
                        return BadRequest(new FailResponse
                        {
                            Message = string.Format(ProductsAreNotAvailableWithThisCount, productPackageFlavourFromDb.Quantity)
                        });
                }

                cartProduct.Price = productPackageFlavourFromDb.Price;

                await CalculateTotalAmounts(isSubtracting: false,
                    cart,
                    cartProduct,
                    productFromDb);

                await EnablePromoCode(cart);

                await SetSessionCart(cart);

                var product = await db.Products
                    .FirstAsync(x => x.ProductId == productPackageFlavourFromDb.ProductId);

                return Ok(new
                {
                    product.Name
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Cart/Remove")]
        public async Task<ActionResult> RemoveFromCart([FromBody] CartProductServiceModel productToRemove)
        {
            CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

            await DisablePromoCode(cart, cart.Code ?? "");

            if (cart.CartProducts == null)
                cart.CartProducts = new List<CartProductServiceModel>();

            var existingProduct = GetExistingProduct(cart, productToRemove);

            if (existingProduct != null)
            {
                var productPackageFlavourFromDb = await GetProductPackageFlavourFromDb(productToRemove);

                if (productPackageFlavourFromDb == null)
                    return BadRequest(new FailResponse
                    {
                        Message = ProductDoesNotExists
                    });

                existingProduct.Count -= productToRemove.Count;

                if (existingProduct.Count <= 0)
                {
                    cart.CartProducts?.Remove(existingProduct);
                }

                var productFromDb = await db.Products
                    .FirstAsync(x => x.ProductId == productPackageFlavourFromDb.ProductId);

                productToRemove.Price = productPackageFlavourFromDb.Price;

                await CalculateTotalAmounts(isSubtracting: true,
                    cart,
                    productToRemove,
                    productFromDb);

                await EnablePromoCode(cart);

                await SetSessionCart(cart);

                return Ok();
            }

            return NotFound(new FailResponse
            {
                Message = ProductNotFound
            });
        }

        [HttpPost]
        [Route("/Cart/ApplyPromoCode")]
        public async Task<ActionResult<bool>> ApplyPromoCode([FromBody] ApplyPromoCodeServiceModel promoCodeModel)
        {
            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.TotalProducts == 0)
                {
                    return BadRequest(new FailResponse
                    {
                        Message = YouHaveToAddProducts
                    });
                }

                var promoCode = await db.PromoCodes
                    .FirstOrDefaultAsync(x => x.Code == promoCodeModel.Code);

                if (promoCode == null)
                    return BadRequest(new FailResponse
                    {
                        Key = "PromoCode",
                        Message = InvalidPromoCode
                    });

                await DisablePromoCode(cart);

                cart.TotalProducts -= promoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved += promoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.Code = promoCode.Code;

                await db.SaveChangesAsync();

                await SetSessionCart(cart);

                return Ok(true);
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpPost]
        [Route("/Cart/RemovePromoCode")]
        public async Task<ActionResult<bool>> RemovePromoCode([FromBody] ApplyPromoCodeServiceModel promoCodeModel)
        {
            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                //if (cart.TotalProducts == 0)
                //{
                //    return BadRequest(new FailResponse
                //    {
                //        Message = YouHaveToAddProducts
                //    });
                //}
                var promoCode = await db.PromoCodes
                    .FirstOrDefaultAsync(x => x.Code == promoCodeModel.Code);

                if (promoCode == null)
                    return BadRequest(new FailResponse
                    {
                        Key = "PromoCode",
                        Message = InvalidPromoCode
                    });

                await DisablePromoCode(cart);
                cart.Code = "";

                await db.SaveChangesAsync();

                await SetSessionCart(cart);

                return Ok(true);
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpDelete]
        [Route("/Cart/Clean")]
        public async Task<ActionResult> CleanSessionCart()
        {
            try
            {
                var cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.TotalProducts == 0)
                    return BadRequest(new FailResponse
                    {
                        Message = CartIsAlreadyEmpty
                    });

                await SetSessionCart(new CartServiceModel());

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private CartServiceModel? GetSessionCart()
        {
            string cookieValue = Request.Cookies[CartCookieName]!; // be aware

            if (string.IsNullOrEmpty(cookieValue))
                return new CartServiceModel();

            var cart = JsonConvert.DeserializeObject<CartServiceModel>(cookieValue);
            return cart;
        }

        private async Task SetSessionCart(CartServiceModel cart)
        {
            await Task.Run(() =>
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // this was commented to show the cookie as I type document.cookie
                    Expires = DateTime.UtcNow.AddDays(7),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                string serializedCart = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append(CartCookieName, serializedCart, cookieOptions);
            });
        }

        private async Task DisablePromoCode(CartServiceModel cart, string code = "")
        {
            if (!string.IsNullOrEmpty(code))
            {
                var prevPromoCode = await db.PromoCodes
                .FirstAsync(x => x.Code == code);

                cart.TotalProducts += prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved -= prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                return;
            }

            if (!string.IsNullOrEmpty(cart.Code))
            {
                var prevPromoCode = await db.PromoCodes
                .FirstAsync(x => x.Code == cart.Code);

                cart.TotalProducts += prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved -= prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
            }
        }

        private async Task EnablePromoCode(CartServiceModel cart)
        {
            if (!string.IsNullOrEmpty(cart.Code))
            {
                var prevPromoCode = await db.PromoCodes
                .FirstAsync(x => x.Code == cart.Code);

                cart.TotalProducts -= prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved += prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
            }
        }

        private async Task CalculateTotalAmounts(bool isSubtracting,
                CartServiceModel cart,
                CartProductServiceModel cartProduct,
                Product productFromDb)
        {
            if (isSubtracting)
            {
                if (productFromDb.PromotionId != null)
                {
                    var promotion = await db.Promotions
                        .FirstAsync(x => x.PromotionId == productFromDb.PromotionId);

                    if (promotion.DiscountPercentage != null)
                    {
                        cart.TotalProducts -= (decimal)cartProduct.Price! * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count; // be aware
                        cart.OriginalPrice -= (decimal)cartProduct.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.TotalSaved -= (decimal)promotion.DiscountPercentage / 100 * (decimal)cartProduct.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalProducts -= ((decimal)cartProduct.Price! - (decimal)promotion.DiscountAmount) * cartProduct.Count; // be aware
                        cart.OriginalPrice -= ((decimal)cartProduct.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.TotalSaved -= (decimal)promotion.DiscountAmount * cartProduct.Count;
                    }
                }
                else
                {
                    cart.TotalProducts -= (decimal)cartProduct.Price! * cartProduct.Count;
                    cart.OriginalPrice -= (decimal)cartProduct.Price * cartProduct.Count;
                }
            }
            else
            {
                if (productFromDb.PromotionId != null)
                {
                    var promotion = await db.Promotions
                        .FirstAsync(x => x.PromotionId == productFromDb.PromotionId);

                    if (promotion.DiscountPercentage != null)
                    {
                        cart.TotalProducts += (decimal)cartProduct.Price! * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count; // be aware
                        cart.OriginalPrice += (decimal)cartProduct.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.TotalSaved += (decimal)promotion.DiscountPercentage / 100 * (decimal)cartProduct.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalProducts += ((decimal)cartProduct.Price! - (decimal)promotion.DiscountAmount) * cartProduct.Count; // be aware
                        cart.OriginalPrice += ((decimal)cartProduct.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.TotalSaved += (decimal)promotion.DiscountAmount * cartProduct.Count;
                    }
                }
                else
                {
                    cart.TotalProducts += (decimal)cartProduct.Price! * cartProduct.Count;
                    cart.OriginalPrice += (decimal)cartProduct.Price * cartProduct.Count;
                }
            }

            if (cart.TotalSaved < 0)
                cart.TotalSaved = 0;
            if (cart.TotalProducts < 0)
                cart.TotalProducts = 0;
            if (cart.OriginalPrice < 0)
                cart.OriginalPrice = 0;
        }

        private static CartProductServiceModel? GetExistingProduct(CartServiceModel cart, CartProductServiceModel cartProduct)
            => cart.CartProducts
                    .FirstOrDefault(i => i.ProductId == cartProduct.ProductId &&
                    i.Flavour == cartProduct.Flavour &&
                    i.Grams == cartProduct.Grams);

        private async Task<ProductPackageFlavour?> GetProductPackageFlavourFromDb(CartProductServiceModel cartProduct)
            => await db.ProductsPackagesFlavours
                    .FirstOrDefaultAsync(x => x.ProductId == cartProduct.ProductId &&
                    x.Flavour!.FlavourName == cartProduct.Flavour
                    && x.Package!.Grams == cartProduct.Grams);

        private bool CanRemoveProduct(int? quantity, int wantedAmount)
            => quantity >= wantedAmount;
    }
}