using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VacationManagementApi.Data;
using VacationManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("no"),
    new CultureInfo("sv")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Note: Not recommended, but done to prevent circular/bi-directional reference.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    })
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

builder.Services.AddDbContext<VacationDbContext>(options =>
{
    options.UseInMemoryDatabase("VacationDb");
});

builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<VacationRequestService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VacationDbContext>();
    DatabaseInitializer.Initialize(db);
}

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRouting();
app.MapControllers();

app.UseStaticFiles();
app.MapFallbackToFile("/app/{*path:nonfile}", "/app/index.html");

app.Run();
