using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Queries.User;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Handlers.User
{
    public class GetUserByNameHandler : IRequestHandler<GetUserByNameQuery, IDataResult<ProductPortal.Core.Entities.Aggregates.User>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserByNameHandler(IUserRepository userRepository, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IDataResult<Core.Entities.Aggregates.User>> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var getUserByUsername = await _userRepository.GetByUserName(request.Username);
                if (getUserByUsername is null)
                {
                    return new ErrorDataResult<Core.Entities.Aggregates.User>(
                        _logger,
                        _httpContextAccessor,
                        "Kullanici Bulunamadi!");
                }
                return new SuccessDataResult<Core.Entities.Aggregates.User>(
                       _logger,
                       _httpContextAccessor,
                       getUserByUsername,
                       "Kullanıcı başarıyla bulundu.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Core.Entities.Aggregates.User>(
                        _logger,
                        _httpContextAccessor,
                        "Kullanıcı bilgisi alınamadı");
            }
        }
    }
}
