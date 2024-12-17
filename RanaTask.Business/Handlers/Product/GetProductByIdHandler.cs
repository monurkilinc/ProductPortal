using MediatR;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Queries.Product;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Handlers.Product
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, IDataResult<ProductPortal.Core.Entities.Aggregates.Product>>
    {
        private readonly IProductService _productService;

        public GetProductByIdHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IDataResult<ProductPortal.Core.Entities.Aggregates.Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetByIdAsync(request.Id);
        }
    }
}
