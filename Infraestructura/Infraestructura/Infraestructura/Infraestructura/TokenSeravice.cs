namespace Infraestructura;

public class TokenService
{
    public string GenerateToken(string userId)
        => $"dummy-token-{userId}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
}
