using Microsoft.AspNetCore.Authentication.Cookies;
using MyIntegratedApp.Helpers;
using NETCore.MailKit.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbHelperService, DbHelperService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; // Or your actual login path
        options.AccessDeniedPath = "/Home/AccessDenied";
    });



builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.UseStatusCodePages(async context =>
//{
//    if (context.HttpContext.Response.StatusCode == 403)
//    {
//        var emailService = context.HttpContext.RequestServices.GetRequiredService<IEmailService>();
//        var user = context.HttpContext.User.Identity?.Name ?? "Anonymous";
//        var path = context.HttpContext.Request.Path;
//        await emailService.SendUnauthorizedAttemptEmail(user, path);
//    }
//});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();
