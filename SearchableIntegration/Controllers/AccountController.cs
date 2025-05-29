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
    //        await HttpContext.SignInAsync(
    //CookieAuthenticationDefaults.AuthenticationScheme,
    //principal,
    //new AuthenticationProperties
    //{
    //    IsPersistent = true,
    //    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1)
    //});

            return Ok(new { message = "Login successful",status="success" });

    }

    // POST: /api/account/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}   

}
