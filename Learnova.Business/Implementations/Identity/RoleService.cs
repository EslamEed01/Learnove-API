using AutoMapper;
using AutoMapper.QueryableExtensions;
using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Roles;
using Learnova.Business.Errors;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Learnova.Business.Identity
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, LearnoveDbContext context, IMapper mapper) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly LearnoveDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default) =>
            await _roleManager.Roles
                      .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
                     .ProjectTo<RoleResponse>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);

        public async Task<Abstraction.Result<RoleDetailResponse>> GetAsync(string id)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims
                .Where(c => c.Type == Permissions.Type)
                .Select(c => c.Value);

            var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions);

            return Result.Success(response);
        }

        public async Task<Abstraction.Result<RoleDetailResponse>> AddAsync(RoleRequest request)
        {
            var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);

            if (roleIsExists)
                return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

            var allowedPermissions = Permissions.GetAllPermissions().Where(p => !string.IsNullOrEmpty(p)).Cast<string>();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                foreach (var permission in request.Permissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
                }

                var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
                return Result.Success(response);
            }

            var error = result.Errors.First();
            return Result.Failure<RoleDetailResponse>(new Abstraction.Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UpdateAsync(string id, RoleRequest request)
        {
            var roleIsExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

            if (roleIsExists)
                return Result.Failure(RoleErrors.DuplicatedRole);

            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure(RoleErrors.RoleNotFound);

            var allowedPermissions = Permissions.GetAllPermissions().Where(p => !string.IsNullOrEmpty(p)).Cast<string>();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure(RoleErrors.InvalidPermissions);

            role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var currentPermissions = currentClaims
                    .Where(c => c.Type == Permissions.Type)
                    .Select(c => c.Value)
                    .ToList();

                var permissionsToRemove = currentPermissions.Except(request.Permissions);
                foreach (var permission in permissionsToRemove)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(Permissions.Type, permission));
                }

                // Add new permissions
                var permissionsToAdd = request.Permissions.Except(currentPermissions);
                foreach (var permission in permissionsToAdd)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
                }

                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Abstraction.Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;
            await _roleManager.UpdateAsync(role);

            return Result.Success();
        }
    }
}
