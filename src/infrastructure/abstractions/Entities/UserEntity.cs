using System;

namespace Shipstone.Pollen.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user.
/// </summary>
public class UserEntity : Entity<Guid>
{
    private String _emailAddress;
    private String _emailAddressNormalized;
    private String _passwordHash;

    /// <summary>
    /// Gets or initializes the email address of the user.
    /// </summary>
    /// <value>The email address of the user.</value>
    /// <exception cref="ArgumentNullException">The property is initialized and the value is <c>null</c>.</exception>
    public String EmailAddress
    {
        get => this._emailAddress;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._emailAddress = value;
        }
    }

    /// <summary>
    /// Gets or initializes the normalized email address of the user.
    /// </summary>
    /// <value>The normalized email address of the user.</value>
    /// <exception cref="ArgumentNullException">The property is initialized and the value is <c>null</c>.</exception>
    public String EmailAddressNormalized
    {
        get => this._emailAddressNormalized;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._emailAddressNormalized = value;
        }
    }

    /// <summary>
    /// Gets or sets the password hash of the user.
    /// </summary>
    /// <value>The hashed representation of the password of the user.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String PasswordHash
    {
        get => this._passwordHash;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._passwordHash = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEntity" />.
    /// </summary>
    public UserEntity()
    {
        this._emailAddress = String.Empty;
        this._emailAddressNormalized = String.Empty;
        this._passwordHash = String.Empty;
    }
}
