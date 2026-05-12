using EShoppingZone.Wallet.API.DTOs;
using EShoppingZone.Wallet.API.Entities;
using EShoppingZone.Wallet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShoppingZone.Wallet.API.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // POST /api/wallet/new
        [HttpPost("new")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddNewWallet()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var wallet = new EWallet
            {
                WalletId = int.Parse(userIdClaim),
                CurrentBalance = 0
            };

            var created = await _walletService.AddWallet(wallet);
            return Ok(created);
        }

        // GET /api/wallet
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllWallets()
        {
            var wallets = await _walletService.GetWallets();
            return Ok(wallets);
        }

        // GET /api/wallet/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var wallet = await _walletService.GetById(id);
            if (wallet == null) return NotFound("Wallet not found.");
            return Ok(wallet);
        }

        // POST /api/wallet/addMoney
        [HttpPost("addMoney")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddMoney([FromBody] AddMoneyDto dto)
        {
            await _walletService.AddMoney(dto.WalletId, dto.Amount, dto.Remarks);
            return Ok("Money added successfully.");
        }

        // POST /api/wallet/payMoney
        [HttpPost("payMoney")]
        [Authorize]
        public async Task<IActionResult> PayMoney([FromBody] PayMoneyDto dto)
        {
            var success = await _walletService.PayMoney(dto.WalletId, dto.Amount, dto.Remarks, dto.OrderId);
            if (!success) return BadRequest(new { message = "Insufficient wallet balance." });
            return Ok(new { message = "Payment successful." });
        }

        // GET /api/wallet/statements/{id}
        [HttpGet("statements/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStatementsById(int id)
        {
            var statements = await _walletService.GetStatementsById(id);
            return Ok(statements);
        }

        // GET /api/wallet/statements
        [HttpGet("statements")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetStatements()
        {
            var statements = await _walletService.GetStatements();
            return Ok(statements);
        }

        // POST /api/wallet/refund
        [HttpPost("refund")]
        [Authorize]
        public async Task<IActionResult> RefundMoney([FromBody] RefundDto dto)
        {
            try
            {
                await _walletService.RefundMoney(dto.WalletId, dto.Amount, dto.Remarks);
                return Ok(new { message = "Refund processed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/wallet/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _walletService.DeleteById(id);
            return Ok(new { message = "Wallet deleted." });
        }

        // POST /api/wallet/initiateTopUp
        [HttpPost("initiateTopUp")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> InitiateTopUp([FromBody] InitiateWalletTopUpDto dto)
        {
            try
            {
                var result = await _walletService.InitiateTopUp(dto.Amount, dto.Currency);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/wallet/verifyAndAdd
        [HttpPost("verifyAndAdd")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> VerifyAndAdd([FromBody] VerifyWalletTopUpDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            try
            {
                await _walletService.VerifyAndAddMoney(int.Parse(userIdClaim), dto);
                return Ok(new { message = "Wallet topped up successfully via online payment." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
