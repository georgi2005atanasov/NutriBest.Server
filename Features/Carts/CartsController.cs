namespace NutriBest.Server.Features.Carts
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.PromoCodes;

    public class CartsController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;
        private readonly ICartService cartService;
        private readonly IPromoCodeService promoCodeService;
        private readonly IMapper mapper;

        public CartsController(ICartService cartService,
            NutriBestDbContext db,
            IPromoCodeService promoCodeService,
            IMapper mapper)
        {
            this.cartService = cartService;
            this.promoCodeService = promoCodeService;
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/cart/get")]
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
                            Price = x.Price,
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
        [Route("/cart/set")]
        public async Task<ActionResult> SetProductCount([FromBody] CartProductServiceModel cartProduct)
        {
            if (cartProduct.Count <= 0)
            {
                return BadRequest(new
                {
                    Message = "Invalid product count!"
                });
            }

            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.CartProducts == null)
                    cart.CartProducts = new List<CartProductServiceModel>();

                var existingProduct = GetExistingProduct(cart, cartProduct);
                var productFromDb = await GetProductFromDb(cartProduct);

                if (productFromDb == null)
                    return BadRequest(new
                    {
                        Message = "This product does not exist!"
                    });

                if (existingProduct != null)
                {
                    await CalculateTotalAmounts(isSubtracting: true,
                        cart,
                        existingProduct,
                        productFromDb);

                    existingProduct.Count = cartProduct.Count;

                    if (!CanRemoveProduct(productFromDb.Quantity, existingProduct.Count))
                    {
                        return BadRequest(new
                        {
                            Message = $"Sorry, we have {productFromDb.Quantity} units of this product available."
                        });
                    }
                }
                else
                {
                    cart.CartProducts.Add(cartProduct);

                    if (!CanRemoveProduct(productFromDb.Quantity, cartProduct.Count))
                    {
                        return BadRequest(new
                        {
                            Message = $"Sorry, we have {productFromDb.Quantity} units of this product available."
                        });
                    }
                }

                await CalculateTotalAmounts(isSubtracting: false,
                    cart,
                    cartProduct,
                    productFromDb);

                await SetSessionCart(cart);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/cart/add")]
        public async Task<ActionResult<string>> AddSessionCart([FromBody] CartProductServiceModel cartProduct)
        {
            if (cartProduct.Count <= 0)
            {
                return BadRequest(new
                {
                    Message = "Invalid product count!"
                });
            }

            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.CartProducts == null)
                    cart.CartProducts = new List<CartProductServiceModel>();

                await DisablePromoCode(cart);

                var existingProduct = GetExistingProduct(cart, cartProduct);

                var productFromDb = await GetProductFromDb(cartProduct);

                if (productFromDb == null)
                    return BadRequest(new
                    {
                        Message = "This product does not exist!"
                    });

                if (existingProduct != null)
                {
                    existingProduct.Count += cartProduct.Count;

                    if (!CanRemoveProduct(productFromDb.Quantity, existingProduct.Count))
                    {
                        return BadRequest(new
                        {
                            Message = $"Sorry, we have {productFromDb.Quantity} units of this product available."
                        });
                    }
                }
                else
                {
                    cart.CartProducts.Add(cartProduct);

                    if (!CanRemoveProduct(productFromDb.Quantity, cartProduct.Count))
                    {
                        return BadRequest(new
                        {
                            Message = $"Sorry, we have {productFromDb.Quantity} units of this product available."
                        });
                    }
                }

                await CalculateTotalAmounts(isSubtracting: false,
                    cart,
                    cartProduct,
                    productFromDb);

                await EnablePromoCode(cart);

                await SetSessionCart(cart);

                return Ok(new
                {
                    productFromDb.Name
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("/cart/remove")]
        public async Task<ActionResult> RemoveSessionCart([FromBody] CartProductServiceModel productToRemove)
        {
            CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

            await DisablePromoCode(cart);

            if (cart.CartProducts == null)
                cart.CartProducts = new List<CartProductServiceModel>();

            var existingProduct = GetExistingProduct(cart, productToRemove);

            if (existingProduct != null)
            {
                var productFromDb = await GetProductFromDb(productToRemove);

                if (productFromDb == null)
                    return BadRequest(new
                    {
                        Message = "This product does not exist!"
                    });

                existingProduct.Count -= productToRemove.Count;

                if (existingProduct.Count <= 0)
                {
                    cart.CartProducts?.Remove(existingProduct);
                }

                await CalculateTotalAmounts(isSubtracting: true,
                    cart,
                    productToRemove,
                    productFromDb);

                await EnablePromoCode(cart);

                await SetSessionCart(cart);

                return Ok();
            }

            return NotFound(new
            {
                Message = "Product not found in the cart."
            });
        }

        [HttpPost]
        [Route("/cart/apply-promo-code")]
        public async Task<ActionResult<bool>> ApplyPromoCode([FromBody] ApplyPromoCodeServiceModel promoCodeModel)
        {
            try
            {
                CartServiceModel cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.TotalPrice == 0)
                {
                    return BadRequest(new
                    {
                        Message = "You have to add products to the cart!"
                    });
                }

                await DisablePromoCode(cart);

                var promoCode = await db.PromoCodes
                    .FirstOrDefaultAsync(x => x.Code == promoCodeModel.Code);

                if (promoCode == null)
                    return BadRequest(new
                    {
                        Key = "PromoCode",
                        Message = "Invalid promo code!"
                    });

                cart.TotalPrice -= promoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved += promoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.Code = promoCode.Code;

                //promoCode.IsValid = false;

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
        [Route("/cart/clean")]
        public async Task<ActionResult> CleanSessionCart()
        {
            try
            {
                var cart = GetSessionCart() ?? new CartServiceModel();

                if (cart.TotalPrice == 0)
                    return BadRequest(new
                    {
                        Message = "The cart is already empty!"
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
                    Expires = DateTime.Now.AddDays(7),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                string serializedCart = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append(CartCookieName, serializedCart, cookieOptions);
            });
        }

        private async Task DisablePromoCode(CartServiceModel cart)
        {
            if (!string.IsNullOrEmpty(cart.Code))
            {
                var prevPromoCode = await db.PromoCodes
                .FirstAsync(x => x.Code == cart.Code);

                cart.TotalPrice += prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
                cart.TotalSaved -= prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
            }
        }

        private async Task EnablePromoCode(CartServiceModel cart)
        {
            if (!string.IsNullOrEmpty(cart.Code))
            {
                var prevPromoCode = await db.PromoCodes
                .FirstAsync(x => x.Code == cart.Code);

                cart.TotalPrice -= prevPromoCode.DiscountPercentage / 100 * cart.OriginalPrice;
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
                        cart.TotalPrice -= productFromDb.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.OriginalPrice -= productFromDb.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.TotalSaved -= (decimal)promotion.DiscountPercentage / 100 * productFromDb.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalPrice -= (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.OriginalPrice -= (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.TotalSaved -= (decimal)promotion.DiscountAmount * cartProduct.Count;
                    }
                }
                else
                {
                    cart.TotalPrice -= productFromDb.Price * cartProduct.Count;
                    cart.OriginalPrice -= productFromDb.Price * cartProduct.Count;
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
                        cart.TotalPrice += productFromDb.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.OriginalPrice += productFromDb.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100) * cartProduct.Count;
                        cart.TotalSaved += (decimal)promotion.DiscountPercentage / 100 * productFromDb.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalPrice += (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.OriginalPrice += (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.TotalSaved += (decimal)promotion.DiscountAmount * cartProduct.Count;
                    }
                }
                else
                {
                    cart.TotalPrice += productFromDb.Price * cartProduct.Count;
                }
            }
        }

        private static CartProductServiceModel? GetExistingProduct(CartServiceModel cart, CartProductServiceModel cartProduct)
            => cart.CartProducts
                    .FirstOrDefault(i => i.ProductId == cartProduct.ProductId &&
                    i.Flavour == cartProduct.Flavour &&
                    i.Grams == cartProduct.Grams);


        private async Task<Product?> GetProductFromDb(CartProductServiceModel cartProduct)
            => await db.Products
                    .FirstOrDefaultAsync(x => x.ProductId == cartProduct.ProductId &&
                    x.ProductPackageFlavours.Any(y => y.Flavour!.FlavourName == cartProduct.Flavour
                    && x.ProductPackageFlavours.Any(y => y.Package!.Grams == cartProduct.Grams)));

        private bool CanRemoveProduct(int? quantity, int count)
            => quantity >= count;
    }
}