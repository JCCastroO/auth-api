namespace Auth.Api.Model.Entities;

public class UserEntity(Guid id, string name, string email, string password)
{
    public Guid Id { get; init; } = id;

    public string Name { get; init; } = name;

    public string Email { get; private set; } = email;

    public string Password { get; private set; } = password;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; } = default;
}