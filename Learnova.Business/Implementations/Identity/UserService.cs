using AutoMapper;
using AutoMapper.QueryableExtensions;
using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Users;
using Learnova.Business.Errors;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Identity
{
    public class UserService(UserManager<AppUser> userManager,
   IRoleService roleService,
   LearnoveDbContext context, IMapper mapper) : IUserService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly LearnoveDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await (from u in _context.Users
               join ur in _context.UserRoles
               on u.Id equals ur.UserId
               join r in _context.Roles
               on ur.RoleId equals r.Id into roles
               select new
               {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.RoleType,
                   u.IsDisabled,
                   Roles = roles.Select(x => x.Name!).ToList()
               }
                )
                .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.RoleType, u.IsDisabled })
                .Select(u => new UserResponse
                (
                    u.Key.Id,
                    u.Key.FirstName,
                    u.Key.LastName,
                    u.Key.Email,
                    u.Key.RoleType,
                    u.Key.IsDisabled,
                    u.SelectMany(x => x.Roles).ToList()
                ))
               .ToListAsync(cancellationToken);

        public async Task<Result<UserResponse>> GetAsync(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            var userRoles = await _userManager.GetRolesAsync(user);

            var response = _mapper.Map<UserResponse>(new UserWithRoles
            {
                User = user,
                Roles = userRoles
            });
            return Result.Success(response);
        }

        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.email, cancellationToken);

            if (emailIsExists)
                return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

            var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

            var user = _mapper.Map<AppUser>(request);

            var result = await _userManager.CreateAsync(user, request.password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, request.Roles);

                var response = _mapper.Map<UserResponse>(new UserWithRoles { User = user, Roles = request.Roles });

                return Result.Success(response);
            }

            var error = result.Errors.First();

            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.email && x.Id != id, cancellationToken);

            if (emailIsExists)
                return Result.Failure(UserErrors.DuplicatedEmail);

            var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure(UserErrors.InvalidRoles);

            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            _mapper.Map(request, user);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _context.UserRoles
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await _userManager.AddToRolesAsync(user, request.Roles);

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ToggleStatus(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> Unlock(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsDisabled = false;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == userId)
               .ProjectTo<UserProfileResponse>(_mapper.ConfigurationProvider)
                .SingleAsync();

            return Result.Success(user);
        }

        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            await _userManager.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(x => x.FirstName, request.FirstName)
                        .SetProperty(x => x.LastName, request.LastName)
                );

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


    }
}
