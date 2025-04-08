using BuildingBlocks;
using Catalog.API.Data;
using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddCore(false);
builder.Services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCore(false);
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.UseMigration();
app.Run();
