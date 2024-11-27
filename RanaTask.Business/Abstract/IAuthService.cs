using Azure.Core;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Interfaces;

namespace ProductPortal.Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<User>> RegisterAsync(UserCreateDTO createDto);
        Task<IDataResult<LoginResponse>> LoginAsync(UserLoginDTO userLoginDTO);
        Task<IDataResult<Core.Utilities.Security.AccessToken>> CreateAccessTokenAsync(User user);
        Task<IDataResult<User>> GetByUsernameAsync(string username);    
    }
}
