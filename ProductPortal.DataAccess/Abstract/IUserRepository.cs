using ProductPortal.Core.Entities.Aggregates;

namespace ProductPortal.DataAccess.Abstract
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<User> GetByUserName(string userName);
        Task<User> GetByEmail(string email);
        Task<List<User>> GetUsersByRoleAsync(string role);
        Task<int> GetTotalCountAsync();
        Task DeleteAsync(User user);
        Task<int> GetCountAsync();
        Task<List<User>> GetByRoleAsync(string role);
    }
}
