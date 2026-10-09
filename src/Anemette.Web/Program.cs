using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Database (MySQL hos Simply.com). Forbindelsen står i appsettings.Production.json på serveren, aldrig i Git.
var forbindelse = config.GetConnectionString("Database")
    ?? throw new InvalidOperationException("ConnectionStrings:Database mangler. Se README.md.");
builder.Services.AddSingleton(sp => FeltKryptering.FraKonfiguration(config, builder.Environment));
builder.Services.AddDbContext<AppDbContext>(o => o.UseMySQL(forbindelse));

builder.Services.AddDataProtection()
    .SetApplicationName("Anemette")
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/admin/login";
        o.LogoutPath = "/admin/logud";
        o.AccessDeniedPath = "/admin/login";
        o.Cookie.Name = "anemette-admin";
        o.Cookie.HttpOnly = true;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddRazorPages(o =>
{
    o.Conventions.AuthorizeFolder("/Admin");
    o.Conventions.AllowAnonymousToPage("/Admin/Login");
}).AddMvcOptions(o =>
{
    // Alle obligatoriske felter har danske [Required]-tekster; undgå automatiske engelske beskeder
    o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    var m = o.ModelBindingMessageProvider;
    m.SetValueMustBeANumberAccessor(felt => $"«{felt}» skal være et tal.");
    m.SetAttemptedValueIsInvalidAccessor((vaerdi, felt) => $"«{vaerdi}» er ikke gyldig i «{felt}».");
    m.SetValueMustNotBeNullAccessor(felt => $"«{felt}» skal udfyldes.");
    m.SetMissingBindRequiredValueAccessor(felt => $"«{felt}» mangler.");
    m.SetUnknownValueIsInvalidAccessor(felt => $"Værdien i «{felt}» er ikke gyldig.");
    m.SetNonPropertyAttemptedValueIsInvalidAccessor(vaerdi => $"«{vaerdi}» er ikke gyldig.");
});
// Skriv æ, ø, å og andre tegn direkte i HTML i stedet for som &#xF8; osv.
builder.Services.Configure<Microsoft.Extensions.WebEncoders.WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
builder.Services.AddAntiforgery(o => o.Cookie.Name = "anemette-sikkerhed");
builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<SmtpIndstillinger>(config.GetSection("Smtp"));
builder.Services.AddScoped<Indstillinger>();
builder.Services.AddScoped<Kurv>();
builder.Services.AddScoped<Mail>();
builder.Services.AddScoped<Bestilling>();
builder.Services.AddScoped<AdminLogin>();

// Bogforsider og andre billeder ligger i mappen Uploads ved siden af appen (bevares ved hver udgivelse)
var uploads = new UploadMappe(Path.GetFullPath(config["Uploads:Mappe"] ?? "Uploads", builder.Environment.ContentRootPath));
Directory.CreateDirectory(uploads.Sti);
builder.Services.AddSingleton(uploads);

// Højst 20 login-forsøg pr. 5 minutter fra samme adresse
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "ukendt",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(5) }));
});

// Billeder større end 10 MB afvises
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o => o.MultipartBodyLengthLimit = 10 * 1024 * 1024);

var app = builder.Build();

// IIS sidder foran appen (out-of-process), så den rigtige adresse og https kommer fra proxy-headers
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/fejl");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/fejl", "?kode={0}");

app.Use(async (ctx, next) =>
{
    var h = ctx.Response.Headers;
    h["X-Content-Type-Options"] = "nosniff";
    h["Referrer-Policy"] = "strict-origin-when-cross-origin";
    h["X-Frame-Options"] = "DENY";
    // Alt indhold (skrifttyper, billeder, css) ligger på siden selv – intet hentes fra andre firmaer
    h["Content-Security-Policy"] = "default-src 'self'; img-src 'self' data:; style-src 'self'; script-src 'self'; frame-ancestors 'none'; form-action 'self'";
    await next();
});

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploads.Sti),
    RequestPath = "/Uploads",
    OnPrepareResponse = c => c.Context.Response.Headers.CacheControl = "public,max-age=604800",
});

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Adresser fra den gamle hjemmeside, så gamle links og søgeresultater stadig virker
foreach (var (gammel, ny) in new[]
{
    ("/Produkter/{**rest}", "/boeger"), ("/Produkt/{**rest}", "/boeger"),
    ("/Event/{**rest}", "/naturvandringer"), ("/Home/ContactForm", "/kontakt"),
    ("/Home/Login", "/admin/login"), ("/Home/{**rest}", "/"), ("/Admin/{**rest}", "/admin"),
})
{
    app.MapGet(gammel, () => Results.Redirect(ny, permanent: true));
}

await Startdata.KoerAsync(app.Services);
app.Run();
