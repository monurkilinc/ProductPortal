using MediatR;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Queries.Product
{
    public class GetProductByIdQuery:IRequest<IDataResult<ProductPortal.Core.Entities.Aggregates.Product>>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }
}
