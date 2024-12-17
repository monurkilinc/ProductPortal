using MediatR;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Commands.Product;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Business.Handlers.Product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, IResult>
    {
        private readonly IProductService _productService;

        public DeleteProductHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _productService.DeleteAsync(request.Id);

        }
    }
}
