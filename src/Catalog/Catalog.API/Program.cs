using Catalog.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Services 
var assembly = typeof(Program).Assembly;

builder.AddCore();

builder.Services.AddCarterWithAssemblies();
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

var app = builder.Build();

// Configure HTTP request pipeline
app.UseCore();

app.MapCarter();


app.Run();
