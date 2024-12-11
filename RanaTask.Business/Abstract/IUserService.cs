using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Business.Abstract
{
    public interface IUserService
    {
        Task<IDataResult<User>> CreateUserAsync(UserCreateDTO createDto);
        Task<IDataResult<User>> UpdateUserAsync(UserUpdateDTO updateDto);
        Task<IResult> DeleteUserAsync(int id);

        Task<IDataResult<List<User>>> GetAllUsersAsync();
        Task<IDataResult<User>> GetUserByIdAsync(int id);
        Task<IDataResult<int>> GetTotalUsersCountAsync();
        Task<IDataResult<User>> ToggleUserStatusAsync(int userId);
        Task<IDataResult<User>> GetUserByNameAsync(string username);
        Task<IDataResult<List<User>>> GetUsersByRoleAsync(string role);
    }
}
