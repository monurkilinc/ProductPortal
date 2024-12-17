using MediatR;
using NuGet.Protocol.Plugins;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Queries.Product;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Handlers.Product
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IDataResult<List<Core.Entities.Aggregates.Product>>>
    {
        private readonly IProductService _productService;

        public GetAllProductsHandler(IProductService productService)
        {
            _productService = productService;
        }
      
        public async Task<IDataResult<List<Core.Entities.Aggregates.Product>>>Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetAllAsync();
        }
    }
}
