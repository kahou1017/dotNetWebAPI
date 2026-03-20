using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(JwtTokenRequest request);
}
