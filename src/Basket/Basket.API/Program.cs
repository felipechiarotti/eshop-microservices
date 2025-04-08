using BuildingBlocks;

var builder = WebApplication.CreateBuilder(args);


builder.AddCore();

builder.Services.AddMarten(builder.Configuration, opts =>
{
    opts.Schema.For<ShoppingCart>().Identity(sc => sc.UserName);
});
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

var app = builder.Build();

app.UseCore();

app.Run();
