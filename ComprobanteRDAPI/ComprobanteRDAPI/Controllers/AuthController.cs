using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.DTOs;
using ComprobanteRDAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<User> userManager, AppDbContext context, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            // Check if user already exists
            var existinguser = await userManager.FindByEmailAsync(dto.Email);
            if (existinguser != null)
            {
                return BadRequest("Ya existe un usuario con este Email");
            }

            // Initiate transaction
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // If not exists create the company
                var company = new Company
                {
                    CommercialName = dto.CommercialName,
                    BusinessWhatsAppNumber = dto.BusinessWhatsAppNumber,
                    TaxId = dto.TaxId,
                    SubscriptionPlanId = dto.SubscriptionPlanId > 0 ? dto.SubscriptionPlanId : 1,
                };

                context.Add(company);
                await context.SaveChangesAsync();

                // Create the Admin User 

                var user = new User
                {
                    CompanyId = company.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    UserName = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Active"
                };

                var result = await userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result.Errors);
                }

                await userManager.AddToRoleAsync(user, "Admin");
                await transaction.CommitAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno durante el registro: {ex.Message}");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            // Check if user already exists
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("Credenciales Invalidos");
            }

            // Validate Password
            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Credenciales Invalidos");
            }

            // Get all roles from identity
            var roles = await userManager.GetRolesAsync(user);

            // Creating Claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("CompanyId", user.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add each role as a new claim
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate the JWT
            var token = GenerateJWT(authClaims);

            return Ok(new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                CompanyId = user.CompanyId,
                Expiration = token.ValidTo,
                Roles = roles

            });
        }

        private JwtSecurityToken GenerateJWT(List<Claim> claims)
        {
            var secretKey = configuration["jwtkey"];

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            return new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(8),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

    }
}
