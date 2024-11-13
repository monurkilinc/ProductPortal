using Azure.Core;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Interfaces;

namespace ProductPortal.Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<User>> RegisterAsync(UserRegisterDTO registerDto);
        Task<IDataResult<Core.Utilities.Security.AccessToken>> LoginAsync(UserLoginDTO loginDto);
        Task<IDataResult<Core.Utilities.Security.AccessToken>> CreateAccessTokenAsync(User user);
    }
}
