using API.Middleware;
using Application.Interfaces;
using Application.Services;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services
    .AddControllers(options =>
    {
        // Validation errors use the JSON field names ("nationalId", not "NationalId").
        options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());

        // Only the [Required] attributes we wrote count, so every message is one we chose.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    // Malformed JSON gets a short message instead of the parser's internal details.
    .AddJsonOptions(options => options.AllowInputFormatterExceptionMessages = false);

// Swagger: API documentation at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Grading System API", Version = "v1" }));

// Database: SQL Server through Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();

// Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ILookupService, LookupService>();

var app = builder.Build();

// Create the database and apply any pending migrations on startup,
// so nobody has to run a SQL script or "dotnet ef database update" by hand.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Turns exceptions from the services into JSON errors (400, 404, 409, 500).
app.UseMiddleware<ExceptionMiddleware>();

// Swagger is on in every environment so the evaluator can try the API directly.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
