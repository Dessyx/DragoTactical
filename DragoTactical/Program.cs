using DragoTactical.Models;
using DragoTactical.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DragoTacticalDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(httpContext =>
    {
        IPAddress clientIp = httpContext.Connection.RemoteIpAddress ?? IPAddress.None;
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ =>
        new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2,
            AutoReplenishment = true
        });
    });

    options.AddFixedWindowLimiter("FormSubmissions", options =>
    {
        options.PermitLimit = 2; // 2 submissions per 5 minutes
        options.Window = TimeSpan.FromMinutes(5);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
        options.AutoReplenishment = true;
    });

    options.RejectionStatusCode = 429;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure field encryption key from configuration
try
{
    var encKey = builder.Configuration.GetValue<string>("Encryption:Key");
    EncryptionConfig.SetKeyFromString(encKey);
}
catch (Exception ex)
{
    var tmpLogger = app.Services.GetRequiredService<ILogger<Program>>();
    tmpLogger.LogWarning(ex, "Encryption key not configured correctly; field encryption disabled.");
}

app.Use((ctx, next) =>
{
    ctx.Response.OnStarting(() =>
    {
        var h = ctx.Response.Headers;
        h["Content-Security-Policy"] =
            "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self'; frame-ancestors 'none'";
        h["X-Content-Type-Options"] = "nosniff";
        h["Referrer-Policy"] = "strict-origin-when-cross-origin";
        h["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
        h["X-Frame-Options"] = "DENY";
        return Task.CompletedTask;
    });
    return next();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DragoTacticalDbContext>();
    try
    {
        db.Database.EnsureCreated();
        db.Database.Migrate();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");
        var canConnect = await db.Database.CanConnectAsync();
        var formCount = await db.FormSubmissions.CountAsync();
        var serviceCount = await db.Services.CountAsync();
        logger.LogInformation($"Database connection: {canConnect}, Forms: {formCount}, Services: {serviceCount}");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to initialize database");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
