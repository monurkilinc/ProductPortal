using MediatR;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Queries.Product
{
    public class GetAllProductsQuery : IRequest<IDataResult<List<ProductPortal.Core.Entities.Aggregates.Product>>>
    {
    }
}
