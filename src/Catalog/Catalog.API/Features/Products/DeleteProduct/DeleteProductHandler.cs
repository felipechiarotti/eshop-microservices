namespace Catalog.API.Features.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;
public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required");
    }
}
public class DeleteProductCommandHandler(IDocumentSession session) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        session.Delete<Product>(request.Id);
        await session.SaveChangesAsync(cancellationToken);
        return new DeleteProductResult(true);
    }
}
