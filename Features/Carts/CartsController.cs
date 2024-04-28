namespace NutriBest.Server.Features.Carts
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Carts.Models;

    public class CartsController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;
        private readonly ICartService cartService;

        public CartsController(ICartService cartService,
            NutriBestDbContext db)
        {
            this.cartService = cartService;
            this.db = db;
        }

        [HttpGet]
        [Route("/cart/user/get")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var cart = await cartService.Get();

                return Ok(cart);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/cart/user/add")]
        public async Task<ActionResult> Add(CartProductServiceModel cartProductModel)
        {
            try
            {
                if (cartProductModel.Count <= 0)
                {
                    return BadRequest(new
                    {
                        Message = "Choose the quantity of this product!"
                    });
                }

                var cartProductId = await cartService.Add(cartProductModel.ProductId, cartProductModel.Count);

                return Ok();
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message,
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("/cart/user/remove")]
        public async Task<ActionResult<bool>> Remove(CartProductServiceModel cartProductModel)
        {
            try
            {
                var result = await cartService.Remove(cartProductModel.ProductId, cartProductModel.Count);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Cannot remove unexisting products from the cart!"
                    });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("/cart/user/clean")]
        public async Task<ActionResult> Clean()
        {
            try
            {
                var result = await cartService.Clean();

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "The shopping cart is empty!"
                    });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/cart/guest/get")]
        public async Task<IActionResult> GetCartFromSession()
        {
            return Ok(await GetSessionCart());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/cart/guest/add")]
        public async Task<ActionResult> AddSessionCart([FromBody] CartProductServiceModel cartProduct)
        {
            try
            {
                CartServiceModel cart = await GetSessionCart() ?? new CartServiceModel();

                if (cart.CartProducts == null)
                {
                    cart.CartProducts = new List<CartProductServiceModel>();
                }

                var existingProduct = cart.CartProducts.FirstOrDefault(i => i.ProductId == cartProduct.ProductId);

                var productFromDb = await db.Products
                    .FirstOrDefaultAsync(x => x.ProductId == cartProduct.ProductId);

                if (productFromDb == null)
                {
                    return BadRequest(new
                    {
                        Message = "This product does not exist!"
                    });
                }

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

                cart.TotalPrice += productFromDb.Price * cartProduct.Count;

                await SetSessionCart(cart);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("/cart/guest/remove")]
        public async Task<ActionResult> RemoveSessionCart([FromBody] CartProductServiceModel productToRemove)
        {
            CartServiceModel cart = await GetSessionCart() ?? new CartServiceModel();

            if (cart.CartProducts == null)
            {
                cart.CartProducts = new List<CartProductServiceModel>();
            }

            var existingProduct = cart.CartProducts.FirstOrDefault(i => i.ProductId == productToRemove.ProductId);

            if (existingProduct != null)
            {
                var productFromDb = await db.Products
                    .FirstAsync(x => x.ProductId == existingProduct.ProductId);

                existingProduct.Count -= productToRemove.Count;

                if (existingProduct.Count <= 0)
                {
                    cart.CartProducts?.Remove(existingProduct);
                }

                cart.TotalPrice -= productFromDb.Price * productToRemove.Count;

                await SetSessionCart(cart);

                return Ok();
            }

            return NotFound(new
            {
                Message = "Product not found in the cart."
            });
        }

        [HttpDelete]
        [Route("/cart/guest/clean")]
        public async Task<ActionResult> CleanSessionCart()
        {
            try
            {
                await SetSessionCart(new CartServiceModel());

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private async Task<CartServiceModel?> GetSessionCart()
        {
            return await Task.Run(() =>
            {
                string cookieValue = Request.Cookies[CartCookieName]!; // be aware

                if (string.IsNullOrEmpty(cookieValue))
                {
                    return new CartServiceModel();
                }

                var cart = JsonConvert.DeserializeObject<CartServiceModel>(cookieValue);

                return cart;
            });
        }

        private async Task SetSessionCart(CartServiceModel cart)
        {
            await Task.Run(() =>
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(7)
                };

                string serializedCart = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append(CartCookieName, serializedCart, cookieOptions);
            });
        }

        private bool CanRemoveProduct(int? quantity, int count)
            => quantity >= count;
    }
}
