using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Data.Contracts;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Data.Contracts.Repository;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using Plantilla.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Impl.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _rolesRepository;
        private readonly IUserRoleRepository _userRolesRepository;
        private readonly Dictionary<ErrorType, ErrorObject> _errors;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IRedisCache _redisCache;

        public UserService(
                           IMapper mapper,
                           IUserRepository userRepository,
                           IOptions<Dictionary<ErrorType, ErrorObject>> options,
                           IRoleRepository rolesRepository,
                           IUserRoleRepository userRolesRepository,
                           IUnitOfWork unitOfWork,
                           //IRedisCache redisCache,
                           ILogger<UserService> logger = null)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _rolesRepository = rolesRepository;
            _userRolesRepository = userRolesRepository;
            _errors = options.Value;
            _unitOfWork = unitOfWork;
            //_redisCache = redisCache;
            _logger = logger;
        }

        public async Task<OperationResult<UserDto>> AddUserAsync(UserDto dto)
        {
            var result = new OperationResult<UserDto>();

            try
            {
                if (dto == null)
                {
                    result.AddError(100, "User none");
                }

                var user = new User { Id = Guid.NewGuid(), UserName = dto.UserName };
                await _userRepository.AddAsync(user);

                dto = _mapper.Map<User, UserDto>(user);
                await _unitOfWork.SaveChangesAsync();

                result.SetResult(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                result.AddError(e);
                throw;
            }

            return result;
        }

        public async Task<OperationResult<UserDto>> AddUserByName(string userName)
        {
            OperationResult<UserDto> result = new OperationResult<UserDto>();

            try
            {
                bool usersExist = await _userRepository.AnyAsync(x => x.UserName == userName);
                if (usersExist)
                {
                    result.AddError(_errors[ErrorType.UserAlreadyExists]);
                }
                else
                {
                    var target = new User { Id = Guid.NewGuid(), UserName = userName };
                    await _userRepository.AddAsync(target);
                    await _unitOfWork.SaveChangesAsync();
                    var dto = _mapper.Map<User, UserDto>(target);
                    result.SetResult(dto);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result.AddError(e);
            }
            return result;

        }

        public async Task<OperationResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            OperationResult<IEnumerable<UserDto>> result = new OperationResult<IEnumerable<UserDto>>();

            try
            {
                List<UserDto> userDto = new List<UserDto>();

                //var cacheData = await _redisCache.GetAsync();


                //if (cacheData != null)
                //{
                //    result.SetResult(userDto);
                //    return result;
                //}
                //else
                //{
                //}
                var user = await _userRepository.FindAsync();
                var dtos = _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(user);
                //await _redisCache.SetAsync(dtos);
                result.SetResult(dtos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result.AddError(e);
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> GetUser(Guid userId)
        {
            OperationResult<UserDto> result = new OperationResult<UserDto>();
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    var dto = _mapper.Map<User, UserDto>(user);
                    result.SetResult(dto);
                }
                else
                {

                    result.AddError(_errors[ErrorType.UserNotFound]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result.AddError(e);
            }
            return result;
        }

        public async Task<OperationResult> RemoveUser(Guid userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                _logger.LogInformation("Deleting User");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    await _userRepository.RemoveAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    result.AddError(_errors[ErrorType.UserNotFound]);
                }
            }
            catch (Exception e)
            {
                result.AddError(e);
                _logger.LogError(e, e.Message);
            }
            return result;
        }

        public async Task<OperationResult> SaveUserWithRolesAsync(string username, string password, string roleName)
        {

            var result = new OperationResult();
            var user = new User();
            var hashedPassword = user.SetHashedPassWord(password);

            try
            {
                var userId = Guid.NewGuid();
                var roleId = Guid.NewGuid();

                var userRoles = new UserRole
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    UserId = userId
                };

                var roles = new Role
                {
                    Id = roleId,
                    Name = roleName,
                    Description = "foo"
                };

                var userToWrite = new User
                {
                    Id = userId,
                    HashedPassword = hashedPassword,
                    UserName = username
                };

                await _rolesRepository.AddAsync(roles);
                await _userRepository.AddAsync(userToWrite);
                await _userRolesRepository.AddAsync(userRoles);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.AddError(ex);
            }

            return result;
        }
    }
}
