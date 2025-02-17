using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebApplication1;
using WebApplication1.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders(); ;
// Password policy
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;            // Must have at least one digit (0-9)
    options.Password.RequireLowercase = true;        // Must have a lowercase letter (a-z)
    options.Password.RequireUppercase = true;        // Must have an uppercase letter (A-Z)
    options.Password.RequireNonAlphanumeric = true;  // Must have a special character (!@#$%^&)
    options.Password.RequiredLength = 8;             // Minimum length of 8 characters
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout for 5 minutes
    options.Lockout.MaxFailedAccessAttempts = 3; // Lock after 3 failed attempts
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true; // Require unique email
});
builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 10000; // Adjust hashing strength
});

// Add session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Session timeout after 20 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// audit log service
builder.Services.AddScoped<AuditLogService>();

// session tracker
builder.Services.AddSingleton<SessionTracker>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Home");
    options.Conventions.AuthorizePage("/Privacy");
});

// email service
builder.Services.AddTransient<IEmailService, SendGridEmailService>();

// recaptcha service
builder.Services.AddHttpClient<ReCaptchaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();