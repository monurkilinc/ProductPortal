using MediatR;
using ProductPortal.Core.Utilities.Interfaces;

namespace ProductPortal.Business.Queries.User
{
    public class GetUserByNameQuery:IRequest<IDataResult<ProductPortal.Core.Entities.Aggregates.User>>
    {
        public string Username {  get; set; }

        public GetUserByNameQuery(string userName)
        { 
            Username = userName;
        }
    }
}
