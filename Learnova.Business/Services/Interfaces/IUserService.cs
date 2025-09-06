using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Users;

namespace Learnova.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Abstraction.Result<UserResponse>> GetAsync(string id);
        Task<Abstraction.Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatus(string id);
        Task<Result> Unlock(string id);
        Task<Abstraction.Result<UserProfileResponse>> GetProfileAsync(string userId);
        Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);


    }
}
