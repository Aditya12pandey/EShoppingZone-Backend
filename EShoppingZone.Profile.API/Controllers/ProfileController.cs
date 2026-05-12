using EShoppingZone.Profile.API.DTOs;
using EShoppingZone.Profile.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShoppingZone.Profile.API.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _service;

        public ProfileController(IProfileService service)
        {
            _service = service;
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
        {
            try
            {
                var user = await _service.RegisterCustomerAsync(dto);
                return Ok(new { user.ProfileId, user.FullName, user.Role });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("register/merchant")]
        public async Task<IActionResult> RegisterMerchant([FromBody] RegisterMerchantDto dto)
        {
            try
            {
                var user = await _service.RegisterMerchantAsync(dto);
                return Ok(new { user.ProfileId, user.FullName, user.Role });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _service.LoginAsync(dto);
            if (response == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ProfileUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("address")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto dto)
        {
            var address = await _service.AddAddressAsync(dto);
            return Ok(address);
        }

        [HttpGet("address/{profileId}")]
        [Authorize]
        public async Task<IActionResult> GetAddresses(int profileId)
        {
            var addresses = await _service.GetAddressesAsync(profileId);
            return Ok(addresses);
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}