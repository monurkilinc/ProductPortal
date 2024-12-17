using MediatR;
using ProductPortal.Business.DTOs;
using ProductPortal.Core.Utilities.CQRS;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Queries.Product
{
    public class GetProductQuery : IRequest<IDataResult<List<ProductPortal.Core.Entities.Aggregates.Product>>>
    {
        public int Id { get; set; } 
    }
}
