using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger)
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        logger.LogInformation("GetDiscount called with request: {Request}", request);
        if (string.IsNullOrEmpty(request.ProductName))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ProductName is null or empty"));

        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
        if (coupon == null)
        {
            logger.LogWarning("Coupon not found for product: {ProductName}", request.ProductName);
            throw new RpcException(new Status(StatusCode.NotFound, $"Coupon not found for product: {request.ProductName}"));
        }
        logger.LogInformation("Coupon found for {ProductName}: {@Coupon}", coupon.ProductName, coupon);
        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        logger.LogInformation("CreateDiscount called with request: {Request}", request);

        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Coupon is null"));

        await dbContext.Coupons.AddAsync(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Coupon created with ProductName '{ProductName}: {@Coupon}", coupon.ProductName, coupon);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Coupon is null"));

        dbContext.Coupons.Update(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Coupon updated with ProductName '{ProductName}: {@Coupon}", coupon.ProductName, coupon);
        return coupon.Adapt<CouponModel>();
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Coupon not found for product: {request.ProductName}"));

        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Coupon deleted with ProductName '{ProductName}: {@Coupon}", coupon.ProductName, coupon);
        return new() { Success = true };
    }
}
