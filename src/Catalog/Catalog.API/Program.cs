var builder = WebApplication.CreateBuilder(args);

builder.AddCore();


builder.Services.AddMarten(builder.Configuration);

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

var app = builder.Build();

// Configure HTTP request pipeline
app.UseCore();

app.Run();
