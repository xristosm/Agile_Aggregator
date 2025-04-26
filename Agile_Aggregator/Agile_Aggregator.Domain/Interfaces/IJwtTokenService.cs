namespace Agile_Aggregator.Application.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(string userId, IEnumerable<string> roles);
    }
}