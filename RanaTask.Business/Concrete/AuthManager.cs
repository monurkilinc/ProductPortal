using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.Core.Utilities.Results.ErrorResult;
using ProductPortal.Core.Utilities.Results.SuccessResult;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.Core.Utilities.Security;

namespace ProductPortal.Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenHelper _tokenHelper;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthManager(IUserRepository userRepository, ITokenHelper tokenHelper, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IDataResult<User>> RegisterAsync(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                //Email kontrolü
                var emailExists = await _userRepository.GetByEmail(userRegisterDTO.Email);
                if (emailExists is not null)
                {
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Bu email adresi kullanılmaktadır.");
                }
                //Kullanıcı adı kontrolü
                var usernameExists = await _userRepository.GetByUserName(userRegisterDTO.Username);
                if (usernameExists is not null)
                {
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Bu kullanıcı adı kullanılmaktadır.");
                }

                //Sifre hashleme
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(userRegisterDTO.Password, out passwordHash, out passwordSalt);

                var user = new User
                {
                    Email = userRegisterDTO.Email,
                    Username = userRegisterDTO.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                    Role = "User",
                    CreatedDate = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user, "Kayıt başarılı");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanici kaydi sirasinda bir hata olustu!");
            }
        }

        public async Task<IDataResult<Core.Utilities.Security.AccessToken>> LoginAsync(UserLoginDTO userLoginDTO)
        {
            try
            {
                var user = await _userRepository.GetByUserName(userLoginDTO.Username);
                if (user is null)
                {
                    return new ErrorDataResult<Core.Utilities.Security.AccessToken>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");
                }

                if (!HashingHelper.VerifyPasswordHash(userLoginDTO.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return new ErrorDataResult<Core.Utilities.Security.AccessToken>(_logger, _httpContextAccessor, "Sifre hatali");
                }

                var accessToken =await _tokenHelper.CreateAccessTokenAsync(user);
                return new SuccessDataResult<Core.Utilities.Security.AccessToken>(
                    _logger,
                    _httpContextAccessor,
                    accessToken,
                    "Giriş başarılı"
                    );
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Core.Utilities.Security.AccessToken>(_logger, _httpContextAccessor, "Giriş sırasında bir hata oluştu!");
            }


        }
        public async Task<IDataResult<Core.Utilities.Security.AccessToken>> CreateAccessTokenAsync(User user)
        {
            try
            {
                var accessToken = await _tokenHelper.CreateAccessTokenAsync(user);
                return new SuccessDataResult<Core.Utilities.Security.AccessToken>(
                    _logger,
                    _httpContextAccessor,
                    accessToken,
                    "Token oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token oluşturma sırasında hata oluştu");
                return new ErrorDataResult<Core.Utilities.Security.AccessToken>(
                    _logger,
                    _httpContextAccessor,
                    "Token oluşturulurken bir hata oluştu");
            }
        }
    }
}
