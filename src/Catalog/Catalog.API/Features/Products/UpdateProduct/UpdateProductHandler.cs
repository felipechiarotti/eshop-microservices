namespace Catalog.API.Features.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, IEnumerable<string> Category, string Name, string Description, decimal Price, string ImageUrl) : ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

    }
}
public class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(request.Id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), request.Id.ToString());

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageFile = request.ImageUrl;
        product.Category = request.Category.ToList();

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);


        return new UpdateProductResult(true);
    }
}
