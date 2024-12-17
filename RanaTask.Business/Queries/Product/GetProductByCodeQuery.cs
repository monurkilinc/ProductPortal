using MediatR;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Queries.Product
{
    public class GetProductByCodeQuery : IRequest<IDataResult<ProductPortal.Core.Entities.Aggregates.Product>>
    {
        public string Code{ get; set; }

        public GetProductByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
