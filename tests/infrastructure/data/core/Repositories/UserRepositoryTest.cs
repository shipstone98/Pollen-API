using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

using Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest.Repositories;

public sealed class UserRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly MockNormalizationService _normalization;
    private readonly IUserRepository _repository;

    public UserRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenInfrastructureData();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        MockNormalizationService normalization = new();
        services.AddSingleton<INormalizationService>(normalization);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._normalization = normalization;
        this._repository = provider.GetRequiredService<IUserRepository>();
    }

#region RetrieveAsync methods
    [Fact]
    public async Task TestRetrieveAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_Valid_Contains()
    {
        // Arrange
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String EMAIL_ADDRESS_NORMALIZED = "JOHN.DOE@CONTOSO.COM";

        this._normalization._normalizeFunc = _ => EMAIL_ADDRESS_NORMALIZED;

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    EmailAddress = EMAIL_ADDRESS,
                    EmailAddressNormalized = EMAIL_ADDRESS_NORMALIZED
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                EMAIL_ADDRESS,
                CancellationToken.None
            );

        // Assert
        Assert.NotNull(user);
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
    }

    [Fact]
    public async Task TestRetrieveAsync_Valid_NotContains()
    {
        // Arrange
        this._normalization._normalizeFunc = _ => String.Empty;

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                "john.doe@contoso.com",
                CancellationToken.None
            );

        // Assert
        Assert.Null(user);
    }
#endregion

    [Fact]
    public async Task TestUpdateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.UpdateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public Task TestUpdateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.UpdateAsync(user, CancellationToken.None);

        // Nothing to assert
    }
}
