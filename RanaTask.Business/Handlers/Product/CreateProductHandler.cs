using MediatR;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Commands.Product;
using ProductPortal.Core.Entities.Aggregates;
using ResultsIResult = ProductPortal.Core.Utilities.Results.IResult;

namespace ProductPortal.Business.Handlers.Product
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ResultsIResult>
    {
        private readonly IProductService _productService;

        public CreateProductHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ResultsIResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Core.Entities.Aggregates.Product
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Stock = request.Stock,
                Price = request.Price,
                ImageURL = request.ImageURL,
                CreatedDate = DateTime.Now
            };

            return await _productService.AddAsync(product);
        }
    }
}
