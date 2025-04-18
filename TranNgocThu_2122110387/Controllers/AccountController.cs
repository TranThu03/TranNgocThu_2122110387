////using Microsoft.AspNetCore.Mvc;

////// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

////namespace TranNgocThu_2122110387.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class AccountController : ControllerBase
////    {
////        // GET: api/<AccountController>
////        [HttpGet]
////        public IEnumerable<string> Get()
////        {
////            return new string[] { "value1", "value2" };
////        }

////        // GET api/<AccountController>/5
////        [HttpGet("{id}")]
////        public string Get(int id)
////        {
////            return "value";
////        }

////        // POST api/<AccountController>
////        [HttpPost]
////        public void Post([FromBody] string value)
////        {
////        }

////        // PUT api/<AccountController>/5
////        [HttpPut("{id}")]
////        public void Put(int id, [FromBody] string value)
////        {
////        }

////        // DELETE api/<AccountController>/5
////        [HttpDelete("{id}")]
////        public void Delete(int id)
////        {
////        }
////    }
////}


//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace TranNgocThu_2122110387.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AccountController : ControllerBase
//    {
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            if (request.Username == "admin" && request.Password == "123")
//            {
//                var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.Name, request.Username)
//                };

//                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
//                var principal = new ClaimsPrincipal(identity);

//                await HttpContext.SignInAsync("MyCookieAuth", principal);

//                return Ok("Đăng nhập thành công");
//            }

//            return Unauthorized("Tài khoản hoặc mật khẩu không đúng");
//        }

//        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
//        [HttpGet("profile")]
//        public IActionResult GetProfile()
//        {
//            return Ok($"Xin chào {User.Identity?.Name}");
//        }

//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync("MyCookieAuth");
//            return Ok("Đã đăng xuất");
//        }
//    }

//    public class LoginRequest
//    {
//        public string Username { get; set; }
//        public string Password { get; set; }
//    }
//}





//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using TranNgocThu_2122110387.Data;
//using TranNgocThu_2122110387.Model;

//namespace TranNgocThu_2122110387.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AccountController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public AccountController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] User login)
//        {
//            var user = _context.Users.FirstOrDefault(u =>
//                u.Username == login.Username && u.PasswordHash == login.PasswordHash);

//            if (user == null)
//                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

//            // Tạo Claims
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, user.Username),
//                new Claim(ClaimTypes.Role, user.Role ?? "User")
//            };

//            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var principal = new ClaimsPrincipal(identity);

//            await HttpContext.SignInAsync("MyCookieAuth", principal);

//            return Ok("Đăng nhập thành công");
//        }

//        [Authorize]
//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync("MyCookieAuth");
//            return Ok("Đã đăng xuất");
//        }

//        [Authorize]
//        [HttpGet("profile")]
//        public IActionResult Profile()
//        {
//            var username = User.Identity?.Name;
//            return Ok($"Xin chào {username}");
//        }
//    }
//}




using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranNgocThu_2122110387.Data;
using TranNgocThu_2122110387.Model;

namespace TranNgocThu_2122110387.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User register)
        {
            if (_context.Users.Any(u => u.Username == register.Username))
                return BadRequest("Username đã tồn tại");

            var newUser = new User
            {
                Username = register.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.PasswordHash), // Hash mật khẩu
                Role = register.Role ?? "User"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok("Tạo tài khoản thành công");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == login.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            // Tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return Ok("Đăng nhập thành công");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return Ok("Đã đăng xuất");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var username = User.Identity?.Name;
            return Ok($"Xin chào {username}");
        }
    }
}
