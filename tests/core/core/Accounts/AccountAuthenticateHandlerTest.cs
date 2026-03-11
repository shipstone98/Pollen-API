using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Core.Accounts;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

using Shipstone.Pollen.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.CoreTest.Accounts;

public sealed class AuthenticateHandlerTest
{
    private readonly IAccountAuthenticateHandler _handler;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public AuthenticateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenCore();
        MockPasswordService password = new();
        services.AddSingleton<IPasswordService>(password);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);

        this._handler =
            provider.GetRequiredService<IAccountAuthenticateHandler>();

        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("password", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_PasswordNotCorrect()
    {
        // Arrange
        Exception innerException = new IncorrectPasswordException();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveFunc = _ =>
                new UserEntity
                {
                    PasswordHash = String.Empty
                };

            return users;
        };

        this._password._verifyFunc = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<IncorrectPasswordException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Success_PasswordNotSecure()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveFunc = _ =>
                new UserEntity
                {
                    PasswordHash = String.Empty
                };

            users._updateAction = _ => { };
            return users;
        };

        this._password._verifyFunc = (_, _) => false;
        this._password._hashFunc = _ => String.Empty;
        this._repository._saveAction = () => { };

        // Act
        return this._handler.HandleAsync(
            String.Empty,
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }

    [Fact]
    public Task TestHandleAsync_Valid_Success_PasswordSecure()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveFunc = _ =>
                new UserEntity
                {
                    PasswordHash = String.Empty
                };

            users._updateAction = _ => { };
            return users;
        };

        this._password._verifyFunc = (_, _) => true;
        this._repository._saveAction = () => { };

        // Act
        return this._handler.HandleAsync(
            String.Empty,
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion
#endregion
}
