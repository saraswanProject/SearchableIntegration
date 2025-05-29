// Controllers/AccountController.cs
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyIntegratedApp.Helpers;
using SearchableIntegration.Models;
using System.Security.Claims;
namespace MyIntegratedApp.Controllers
{
    public class AccountController : Controller
       {
           [HttpGet]
           public IActionResult Index()
           {
               return View();
           }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Account");
        }

    }



[Route("api/[controller]")] // This makes the base route /api/account
[ApiController]
public class AccountApiController : ControllerBase
{
    private readonly IDbHelperService _dbHelperService;

    public AccountApiController(IDbHelperService dbHelperService)
    {
        _dbHelperService = dbHelperService;
    }


    // POST: /api/account/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@flag", "e");
        parameters.Add("@Email", model.Email);
        var result = await _dbHelperService.ExecuteQuery<dynamic>("spa_login", parameters, true);

        var user = result.Select(x => new { x.PasswordHash, x.Name, x.Email, x.Role }).FirstOrDefault();

        if (user == null || user.PasswordHash != model.Password)
            return Unauthorized(new { message = "Invalid credentials" });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Ok(new { message = "Login successful",status="success" });

    }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupModel model)
        {
            if (string.IsNullOrEmpty(model.Name) ||
              string.IsNullOrEmpty(model.Email) ||
              string.IsNullOrEmpty(model.Password) ||
              string.IsNullOrEmpty(model.Role))
            {
                return BadRequest(new { message = "All fields are required" });
            }
            if (model.Role != "User" && model.Role != "Admin")
            {
                return BadRequest(new { message = "Invalid role selection" });
            }

            // Validate model
            //if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            //    return BadRequest(new { message = "Email and password are required" });

            // Check if user already exists
            var checkParameters = new DynamicParameters();
            checkParameters.Add("@flag", "e");
            checkParameters.Add("@Email", model.Email);
            var existingUser = await _dbHelperService.ExecuteQuery<dynamic>("spa_login", checkParameters, true);

            if (existingUser.Any())
                return BadRequest(new { message = "Email already registered" });

            // Create new user
            var createParameters = new DynamicParameters();
            createParameters.Add("@flag", "i");
            createParameters.Add("@Name", model.Name);
            createParameters.Add("@Email", model.Email);
            createParameters.Add("@PasswordHash", model.Password); // In production, hash this password!
            createParameters.Add("@Role", model.Role);

            await _dbHelperService.ExecuteQuery<dynamic>("spa_login", createParameters, true);

            return Ok(new { message = "Signup successful", status = "success" });
        }

        public class SignupModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; } 
        }
  
    }   

}
