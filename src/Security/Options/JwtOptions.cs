using System.ComponentModel.DataAnnotations;

namespace Security.Options;

/// <summary>
/// Settings used to generate and validate JWT access tokens.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Configuration section name for JWT options.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    [Required]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret key used for HMAC signing.
    /// </summary>
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets access token expiration in minutes.
    /// </summary>
    [Range(1, 1440)]
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
