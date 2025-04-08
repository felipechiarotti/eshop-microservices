using BuildingBlocks.Util;
using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Discount.Grpc.Data;

public class DiscountContext : DbContext
{
    public DbSet<Coupon> Coupons { get; set; } = default!;

    public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
    {
    }

    protected override async void OnModelCreating(ModelBuilder modelBuilder)
    {
        var coupons = await Assembly.GetExecutingAssembly().ReadEmbeddedFileAsync<IEnumerable<Coupon>>("Coupons.json");

        modelBuilder.Entity<Coupon>().HasData(coupons!);
    }
}
