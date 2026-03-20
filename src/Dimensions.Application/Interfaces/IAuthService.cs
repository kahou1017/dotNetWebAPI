using Dimensions.Contracts.Auth;

namespace Dimensions.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<CurrentAdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken = default);
}
