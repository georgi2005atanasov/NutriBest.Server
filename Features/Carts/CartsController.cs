namespace NutriBest.Server.Features.Carts
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Products.Models;

    public class CartsController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;
        private readonly ICartService cartService;
        private readonly IMapper mapper;

        public CartsController(ICartService cartService,
            NutriBestDbContext db,
            IMapper mapper)
        {
            this.cartService = cartService;
            this.db = db;
            this.mapper = mapper;
        }

        //[HttpGet]
        //[Route("/cart/user/get")]
        //public async Task<ActionResult> Get()
        //{
        //    try
        //    {
        //        var cart = await cartService.Get();

        //        return Ok(cart);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //[Route("/cart/user/add")]
        //public async Task<ActionResult> Add(CartProductServiceModel cartProductModel)
        //{
        //    try
        //    {
        //        if (cartProductModel.Count <= 0)
        //            return BadRequest(new
        //            {
        //                Message = "Choose the quantity of this product!"
        //            });

        //        var cartProductId = await cartService.Add(cartProductModel.ProductId, cartProductModel.Count);

        //        return Ok();
        //    }
        //    catch (InvalidOperationException err)
        //    {
        //        return BadRequest(new
        //        {
        //            Message = err.Message,
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpDelete]
        //[Route("/cart/user/remove")]
        //public async Task<ActionResult<bool>> Remove([FromBody] CartProductServiceModel cartProductModel)
        //{
        //    try
        //    {
        //        var result = await cartService.Remove(cartProductModel.ProductId, cartProductModel.Count);

        //        if (!result)
        //            return BadRequest(new
        //            {
        //                Message = "Cannot remove unexisting products from the cart!"
        //            });

        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpDelete]
        //[Route("/cart/user/clean")]
        //public async Task<ActionResult> Clean()
        //{
        //    try
        //    {
        //        var result = await cartService.Clean();

        //        if (!result)
        //            return BadRequest(new
        //            {
        //                Message = "The shopping cart is empty!"
        //            });

        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

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

                await SetSessionCart(cart);

                return Ok();
            }

            return NotFound(new
            {
                Message = "Product not found in the cart."
            });
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
                        cart.TotalSaved -= (decimal)promotion.DiscountPercentage / 100 * productFromDb.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalPrice -= (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
                        cart.TotalSaved -= (decimal)promotion.DiscountAmount * cartProduct.Count;
                    }
                }
                else
                {
                    cart.TotalPrice -= productFromDb.Price * cartProduct.Count;
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
                        cart.TotalSaved += (decimal)promotion.DiscountPercentage / 100 * productFromDb.Price * cartProduct.Count;
                    }
                    if (promotion.DiscountAmount != null)
                    {
                        cart.TotalPrice += (productFromDb.Price - (decimal)promotion.DiscountAmount) * cartProduct.Count;
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
