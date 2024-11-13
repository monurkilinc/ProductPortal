using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.DataAccess.Abstract
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<User> GetByUserName(string userName);
        Task<User> GetByEmail(string email);
    }
}
