using System;
using Xunit;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.AbstractionsTest.Entities;

public sealed class UserEntityTest
{
    [Fact]
    public void TestEmailAddress_Init_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    EmailAddress = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void TestEmailAddress_Init_Valid()
    {
        // Arrange
        const String EMAIL_ADDRESS = "john.doe@contoso.com";

        // Act
        UserEntity user = new UserEntity
        {
            EmailAddress = EMAIL_ADDRESS
        };

        // Assert
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
    }

    [Fact]
    public void TestEmailAddressNormalized_Init_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    EmailAddressNormalized = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void TestEmailAddressNormalized_Init_Valid()
    {
        // Arrange
        const String EMAIL_ADDRESS_NORMALIZED = "JOHN.DOE@CONTOSO.COM";

        // Act
        UserEntity user = new UserEntity
        {
            EmailAddressNormalized = EMAIL_ADDRESS_NORMALIZED
        };

        // Assert
        Assert.Equal(EMAIL_ADDRESS_NORMALIZED, user.EmailAddressNormalized);
    }

    [Fact]
    public void TestPasswordHash_Set_Invalid()
    {
        // Arrange
        UserEntity user = new();
        String passwordHash = user.PasswordHash;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                user.PasswordHash = null!);

        // Assert
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(passwordHash, user.PasswordHash);
    }

    [Fact]
    public void TestPasswordHash_Set_Valid()
    {
        // Arrange
        const String PASSWORD_HASH = "My password hash";
        UserEntity user = new();

        // Act
        user.PasswordHash = PASSWORD_HASH;

        // Assert
        Assert.Equal(PASSWORD_HASH, user.PasswordHash);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        UserEntity user = new();

        // Assert
        Assert.NotNull(user.EmailAddress);
        Assert.NotNull(user.EmailAddressNormalized);
        Assert.NotNull(user.PasswordHash);
    }
}
