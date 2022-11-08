using MindKey.Shared;

namespace MindKey.Server.Authorization;
public class AuthenticateResponse
{
    public long Id { get; set; }
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public string? Username { get; set; } = default!;
    public string? Token { get; set; } = default!;
    public UserType? UserType { get; set; } = default!;
}