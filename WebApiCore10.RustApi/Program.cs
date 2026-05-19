using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApiCore10.RustApi.Application.Extensions;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models;
using WebApiCore10.RustApi.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// Logging: Serilog
// --------------------------------------------------------
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration) // Load from appsettings.json
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("WebApiCore10", "WebApiCore10.Api");
});

//--------------------------------------------------------
// Versioning
//--------------------------------------------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// --------------------------------------------------------
// Services & DI
// --------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddApplicationRegistrationServices();

//--------------------------------------------------------
//DBContxt 
//--------------------------------------------------------
builder.Services.AddDbContext<DataContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Optional extras
    options.User.RequireUniqueEmail = true;
})
       .AddEntityFrameworkStores<DataContext>()
       .AddDefaultTokenProviders();




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//CorrelationIdMiddleware: generates & sets CorrelationId
app.UseMiddleware<CorrelationIdMiddleware>();

//app.UseSerilogRequestLogging(); // Logs HTTP requests
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.Items["CorrelationId"]?.ToString()!);
    };
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
