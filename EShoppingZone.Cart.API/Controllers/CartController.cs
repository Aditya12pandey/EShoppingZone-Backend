using EShoppingZone.Cart.API.DTOs;
using EShoppingZone.Cart.API.Entities;
using EShoppingZone.Cart.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShoppingZone.Cart.API.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // POST /api/carts/create
        [HttpPost("create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddCart()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var cart = await _cartService.AddCart(int.Parse(userIdClaim));
            return Ok(cart);
        }

        // GET /api/carts
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();
            return Ok(carts);
        }

        // GET /api/carts/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCartById(int id)
        {
            var cart = await _cartService.GetCartById(id);
            if (cart == null) return NotFound("Cart not found.");
            return Ok(cart);
        }

        // POST /api/carts/addItem
        [HttpPost("addItem")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int cartId = int.Parse(userIdClaim);
            var cart = await _cartService.GetCartById(cartId);
            if (cart == null) return NotFound("Cart not found. Please create your cart first.");

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItemEntity
                {
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    MerchantId = dto.MerchantId,
                    CartId = cartId
                });
            }

            var updated = await _cartService.UpdateCart(cart);
            return Ok(updated);
        }

        // DELETE /api/carts/removeItem/{cartItemId}
        [HttpDelete("removeItem/{cartItemId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int cartId = int.Parse(userIdClaim);
            var cart = await _cartService.GetCartById(cartId);
            if (cart == null) return NotFound("Cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (item == null) return NotFound("Item not found in cart.");

            cart.Items.Remove(item);
            var updated = await _cartService.UpdateCart(cart);
            return Ok(updated);
        }

        // DELETE /api/carts/clear
        [HttpDelete("clear")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ClearCart()
        {
            Console.WriteLine("DEBUG: ClearCart requested");
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int cartId = int.Parse(userIdClaim);
            var cart = await _cartService.GetCartById(cartId);
            if (cart == null) return NotFound("Cart not found.");

            Console.WriteLine($"DEBUG: Clearing {cart.Items.Count} items from cart {cartId}");
            cart.Items.Clear();
            var updated = await _cartService.UpdateCart(cart);
            return Ok(updated);
        }

        // PUT /api/carts
        [HttpPut]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateCart([FromBody] CartEntity cart)
        {
            var updated = await _cartService.UpdateCart(cart);
            return Ok(updated);
        }
    }
}