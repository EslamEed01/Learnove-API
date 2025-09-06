using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Roles;

namespace Learnova.Business.Services.Interfaces
{
    public interface IRoleService
    {

        Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);
        Task<Abstraction.Result<RoleDetailResponse>> GetAsync(string id);
        Task<Abstraction.Result<RoleDetailResponse>> AddAsync(RoleRequest request);
        Task<Result> UpdateAsync(string id, RoleRequest request);
        Task<Result> ToggleStatusAsync(string id);
    }
}
