using MediatR;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Queries.Product;

namespace ProductPortal.Business.Handlers.Product
{
    public class GetProductByCodeQuery : IRequest<IDataResult<ProductPortal.Core.Entities.Aggregates.Product>>
    {
        public string Code { get; set; }

        public GetProductByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
