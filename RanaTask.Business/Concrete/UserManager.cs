using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Security;
using Microsoft.AspNetCore.Hosting;

namespace ProductPortal.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserManager(IUserRepository userRepository, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IDataResult<List<User>>> GetUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return new SuccessDataResult<List<User>>(_logger, _httpContextAccessor, users);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<User>>(_logger, _httpContextAccessor, "Kullanıcı listesi alınamadı");
            }
        }

        public async Task<IDataResult<User>> GetUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bilgisi alınamadı");
            }
        }

        public async Task<IDataResult<User>> CreateUserAsync(UserCreateDTO dto)
        {
            try
            {
                string imageUrl = null;
                if (dto.ImageFile != null)
                {
                    imageUrl = await SaveImageAsync(dto.ImageFile);
                }
                var user = new User
                {
                    Username = dto.Username,
                    Password = dto.Password,
                    Email = dto.Email,
                    Department = dto.Department,
                    IsAdmin = dto.IsAdmin,
                    Role = dto.IsAdmin ? "Admin" : "User",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ImageUrl = imageUrl,

                };

                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.AddAsync(user);
                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user, "Kullanıcı oluşturuldu");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı oluşturulamadı");
            }
        }

        public async Task<Core.Utilities.Results.IResult> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return new ErrorResult(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                await _userRepository.DeleteAsync(id);
                return new SuccessResult(_logger, _httpContextAccessor, "Kullanıcı başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı silme işlemi sırasında hata oluştu");
                return new ErrorResult(_logger, _httpContextAccessor, "Kullanıcı silinemedi");
            }
        }

        public async Task<IDataResult<User>> AssignRoleAsync(int userId, string role)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                user.Role = role;
                await _userRepository.UpdateAsync(user);
                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user, "Rol atandı");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Rol atanamadı");
            }
        }

        public async Task<IDataResult<User>> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                user.IsActive = !user.IsActive;
                await _userRepository.UpdateAsync(user);
                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user, "Kullanıcı durumu güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı durumu güncellenemedi");
            }
        }

        public async Task<IDataResult<List<User>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return new SuccessDataResult<List<User>>(_logger, _httpContextAccessor, users);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<User>>(_logger, _httpContextAccessor, "Kullanıcı listesi alınamadı");
            }
        }

        public async Task<IDataResult<User>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");

                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bilgisi alınamadı");
            }
        }

        public async Task<IDataResult<int>> GetTotalUsersCountAsync()
        {
            try
            {
                var count = await _userRepository.GetCountAsync();
                return new SuccessDataResult<int>(_logger, _httpContextAccessor, count);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<int>(_logger, _httpContextAccessor, "Kullanıcı sayısı alınamadı");
            }
        }

        public async Task<IDataResult<User>> UpdateUserAsync(UserUpdateDTO updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(updateDto.Id);
                if (user == null)
                {
                    return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bulunamadı");
                }

                // Yeni resim yüklendiyse
                if (updateDto.ImageFile != null)
                {
                    // Eski resmi sil
                    DeleteImage(user.ImageUrl);

                    // Yeni resmi kaydet
                    user.ImageUrl = await SaveImageAsync(updateDto.ImageFile);
                }

                user.Username = !string.IsNullOrEmpty(updateDto.Username) ? updateDto.Username : user.Username;
                user.Password = !string.IsNullOrEmpty(updateDto.Password) ? updateDto.Password : user.Password;
                user.Email = !string.IsNullOrEmpty(updateDto.Email) ? updateDto.Email : user.Email;
                user.Department = !string.IsNullOrEmpty(updateDto.Department) ? updateDto.Department : user.Department;
                user.Role = updateDto.IsAdmin.HasValue && updateDto.IsAdmin.Value ? "Admin" : "User";
                user.IsActive = updateDto.IsActive;
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    byte[] passwordHash, passwordSalt;
                    HashingHelper.CreatePasswordHash(updateDto.Password, out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                await _userRepository.UpdateAsync(user);
                return new SuccessDataResult<User>(_logger, _httpContextAccessor, user, "Kullanıcı güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı güncellenemedi");
            }
        }
        public async Task<IDataResult<List<User>>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var users = await _userRepository.GetByRoleAsync(role);
                return new SuccessDataResult<List<User>>(_logger, _httpContextAccessor, users);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<User>>(_logger, _httpContextAccessor, "Rol bazlı kullanıcı listesi alınamadı");
            }
        }
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return null;

                // Güvenli dosya adı oluştur
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

                // Uploads klasörünün yolunu al
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Dosya yolunu oluştur
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // URL'i döndür
                return $"/uploads/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim kaydedilirken hata oluştu");
                return null;
            }
        }

        public async Task<IDataResult<User>> GetUserByNameAsync(string username)
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
                return new ErrorDataResult<User>(_logger, _httpContextAccessor, "Kullanıcı bilgisi alınamadı");
            }
        }





        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim silinirken hata oluştu");
            }
        }

    }
}
