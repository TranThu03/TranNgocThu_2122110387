using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TranNgocThu_2122110387.Data;
using TranNgocThu_2122110387.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TranNgocThu_2122110387.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userInput)
        {
            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(userInput.PasswordHash) || userInput.PasswordHash.Length < 6)
            {
                return BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            // Kiểm tra tên người dùng đã tồn tại chưa
            if (_context.Users.Any(u => u.Username == userInput.Username))
                return BadRequest("Tên người dùng đã tồn tại.");

            var user = new User
            {
                Username = userInput.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userInput.PasswordHash)
            };

            // Lưu người dùng vào cơ sở dữ liệu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // POST: api/User/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] User userInput)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userInput.Username);
            // Kiểm tra tên người dùng và mật khẩu
            if (user == null || !BCrypt.Net.BCrypt.Verify(userInput.PasswordHash, user.PasswordHash))
                return Unauthorized("Tên người dùng hoặc mật khẩu không hợp lệ.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // Tạo JWT token
        private string GenerateJwtToken(User user)
        {
            string shortKey = _config["Jwt:Key"]; // Đảm bảo là có trong appsettings.json
            string paddedKey = (shortKey ?? "").PadRight(32, 'x');  // Đảm bảo đủ 32 ký tự

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(paddedKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
