var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/";
    options.ApiPath = "/api";
});
app.Run();
