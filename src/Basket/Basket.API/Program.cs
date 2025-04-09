using BuildingBlocks;
using Discount.Grpc;

var builder = WebApplication.CreateBuilder(args);


builder.AddCore();

builder.Services.AddMarten(builder.Configuration, opts =>
{
    opts.Schema.For<ShoppingCart>().Identity(sc => sc.UserName);
});
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

var discountServiceClient = builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(opts =>
{
    opts.Address = new Uri(builder.Configuration["Grpc:DiscountUrl"]!);
});

if (builder.Environment.IsDevelopment())
{
    discountServiceClient.ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });
}

var app = builder.Build();

app.UseCore();

app.Run();
