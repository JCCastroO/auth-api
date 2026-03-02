namespace Auth.Api.Model.Entities;

public class UserEntity()
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; } = default;
}