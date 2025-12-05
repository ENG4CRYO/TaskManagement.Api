using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public ICollection<TaskEntity>? Tasks { get; set; } 

    public List<RefreshToken>? RefreshTokens { get; set; }
}

