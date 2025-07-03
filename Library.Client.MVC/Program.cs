using System.Security.Claims;
using Library.BusinessRules;
using Library.Client.MVC.services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<WorkerService>();
builder.Services.AddSingleton<LoanService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<BLLoans>(); 
builder.Services.AddSingleton<BLLoanDates>();  
builder.Services.AddSingleton<BLCategories>();
builder.Services.AddSingleton<TaskManager>();


builder.Services.AddAuthentication(options =>
{
   options.DefaultScheme = "UserScheme";
})
.AddCookie("AdminScheme", options =>    
{
    options.LoginPath = "/Auth/LoginAdmin"; // Ruta de login para administradores
    options.Cookie.Name = "AdminAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
})
.AddCookie("UserScheme", options =>
{
    options.LoginPath = "/Auth/Login"; // Ruta de login para usuarios
    options.Cookie.Name = "UserAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(5);
    options.SlidingExpiration = true;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.SignInScheme = "UserScheme";
    googleOptions.Scope.Add("profile");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("AdminScheme"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("UserScheme"));
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ejecutar inmediatamente los métodos de notificación al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var loanService = scope.ServiceProvider.GetRequiredService<LoanService>();
    // await loanService.CheckAndNotifyExpiredLoans();        // Notifica préstamos vencidos
    // await loanService.CheckAndNotifyExpiredSoonLoans();    // Notifica préstamos por vencer pronto
}

// Middleware de autenticación y autorización
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Habilitar autenticación
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginAdmin}/{id?}");


app.Run();
