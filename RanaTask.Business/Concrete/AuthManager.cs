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

        async Task<IDataResult<LoginResponse>> IAuthService.LoginAsync(UserLoginDTO userLoginDTO)
        {
            try
            {
                _logger.LogInformation($"Login attempt for username: {userLoginDTO.Username}");

                var user = await _userRepository.GetByUserName(userLoginDTO.Username);
                if (user is null)
                {
                    _logger.LogWarning($"User not found: {userLoginDTO.Username}");
                    return new ErrorDataResult<LoginResponse>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");
                }

                _logger.LogInformation($"User found. Hash length: {user.PasswordHash?.Length}, Salt length: {user.PasswordSalt?.Length}");
                _logger.LogInformation($"Stored Hash: {Convert.ToBase64String(user.PasswordHash)}");
                _logger.LogInformation($"Stored Salt: {Convert.ToBase64String(user.PasswordSalt)}");

                bool isPasswordValid = HashingHelper.VerifyPasswordHash(userLoginDTO.Password, user.PasswordHash, user.PasswordSalt);
                _logger.LogInformation($"Password verification result: {isPasswordValid}");

                if (!isPasswordValid)
                {
                    _logger.LogWarning($"Invalid password for user: {userLoginDTO.Username}");
                    return new ErrorDataResult<LoginResponse>(_logger, _httpContextAccessor, "Şifre hatalı");
                }

                var accessToken = await _tokenHelper.CreateAccessTokenAsync(user);

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };

                _logger.LogInformation($"Login successful for user: {userLoginDTO.Username}");
                return new SuccessDataResult<LoginResponse>(
                    _logger,
                    _httpContextAccessor,
                    response,
                    "Giriş başarılı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", userLoginDTO.Username);
                return new ErrorDataResult<LoginResponse>(
                    _logger,
                    _httpContextAccessor,
                    "Giriş sırasında bir hata oluştu!"
                );
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

        public async Task<IDataResult<User>> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUserName(username);
                if (user == null)
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username");
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bilgileri alınırken hata oluştu");
            }
        }

       
    }
}
