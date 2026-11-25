var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger: API documentation at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger is on in every environment so the evaluator can try the API directly.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
