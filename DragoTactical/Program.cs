using DragoTactical.Models;
using Microsoft.EntityFrameworkCore;
using DragoTactical.Services;
using SQLitePCL; // initialize SQLCipher bundle

var builder = WebApplication.CreateBuilder(args);

// Initialize SQLCipher bundle for encrypted SQLite support
Batteries_V2.Init();

builder.Services.AddControllersWithViews();

// Build connection string with optional password
var baseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbPassword = builder.Configuration["ConnectionStrings:DefaultPassword"]
                 ?? builder.Configuration["Database:Password"]
                 ?? Environment.GetEnvironmentVariable("DB_PASSWORD");

var effectiveConnectionString = string.IsNullOrWhiteSpace(dbPassword)
    ? baseConnectionString
    : $"{baseConnectionString};Password={dbPassword}";

builder.Services.AddDbContext<DragoTacticalDbContext>(options =>
    options.UseSqlite(effectiveConnectionString));

// Configure field-encryption key for value converters
var fieldKey = builder.Configuration["Database:FieldEncryptionKey"]
              ?? builder.Configuration["ConnectionStrings:FieldEncryptionKey"]
              ?? Environment.GetEnvironmentVariable("FIELD_ENCRYPTION_KEY");
DragoTactical.Services.EncryptionConfig.SetKeyFromString(fieldKey);
var loggerFactory = LoggerFactory.Create(cfg => cfg.AddConsole());
var tmpLogger = loggerFactory.CreateLogger<Program>();
tmpLogger.LogInformation($"Field encryption enabled: {DragoTactical.Services.EncryptionConfig.IsEnabled}");

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DragoTacticalDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.EnsureCreated();
        db.Database.Migrate();
        logger.LogInformation("Database initialized successfully");
        // One-time encryption pass for legacy plaintext values
        if (DragoTactical.Services.EncryptionConfig.IsEnabled)
        {
            var updated = 0;
            var submissions = await db.FormSubmissions.ToListAsync();
            foreach (var s in submissions)
            {
                bool changed = false;
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.FirstName)) { s.FirstName = DragoTactical.Services.FieldEncryption.EncryptString(s.FirstName ?? string.Empty)!; changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.LastName)) { s.LastName = DragoTactical.Services.FieldEncryption.EncryptString(s.LastName ?? string.Empty)!; changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.Email)) { s.Email = DragoTactical.Services.FieldEncryption.EncryptString(s.Email ?? string.Empty)!; changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.PhoneNumber)) { s.PhoneNumber = DragoTactical.Services.FieldEncryption.EncryptString(s.PhoneNumber); changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.CompanyName)) { s.CompanyName = DragoTactical.Services.FieldEncryption.EncryptString(s.CompanyName); changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.Location)) { s.Location = DragoTactical.Services.FieldEncryption.EncryptString(s.Location); changed = true; }
                if (!DragoTactical.Services.FieldEncryption.LooksEncrypted(s.Message)) { s.Message = DragoTactical.Services.FieldEncryption.EncryptString(s.Message); changed = true; }
                if (changed) updated++;
            }
            if (updated > 0)
            {
                await db.SaveChangesAsync();
                logger.LogInformation($"Encrypted legacy plaintext fields for {updated} records");
            }
        }
        var canConnect = await db.Database.CanConnectAsync();
        var formCount = await db.FormSubmissions.CountAsync();
        var serviceCount = await db.Services.CountAsync();
        logger.LogInformation($"Database connection: {canConnect}, Forms: {formCount}, Services: {serviceCount}");
    }
    catch (Exception ex)
    {
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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
