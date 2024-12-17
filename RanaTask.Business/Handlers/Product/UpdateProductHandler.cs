using MediatR;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Commands.Product;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Business.Handlers.Product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, IResult>
    {
        private readonly IProductService _productService;

        public UpdateProductHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var updateProduct = new Core.Entities.Aggregates.Product
            {
                Id = request.Id,
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Stock = request.Stock,
                Price = request.Price,
                ImageURL = request.ImageURL,
                UpdatedDate = DateTime.Now
            };
            return await _productService.UpdateAsync(updateProduct);
        }
    }
}
