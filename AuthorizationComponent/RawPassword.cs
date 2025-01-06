using System.Text.RegularExpressions;

namespace AuthorizationComponent;

public record RawPassword(string Value)
{
    // Minimum eight characters, at least one uppercase letter, one lowercase letter and one number. Example: Password1
    public static Regex Regex { get; } = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$");
}
