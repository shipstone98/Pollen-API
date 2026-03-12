using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shipstone.AspNetCore.Authentication.Basic;
using Shipstone.AspNetCore.Http;
using Shipstone.Extensions.Identity;
using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Weather;
using Shipstone.Pollen.Api.Web;
using Shipstone.Pollen.Api.WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

bool isNcsaCommonLoggingEnabled =
    builder.Configuration.GetValue<bool>("IsNcsaCommonLoggingEnabled");

builder.Services
    .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
    .AddBasic();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        JsonConverter converter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(converter);
    })
    .AddPollenControllers();

builder.Services
    .AddDistributedMemoryCache()
    .AddIdentityExtensions()
    .AddPollenCore()
    .AddPollenInfrastructureData()
    .AddPollenInfrastructureWeather(
        builder.Configuration
            .GetRequiredSection("Weather")
            .Bind
    )
    .AddSingleton<IBasicAuthenticateHandler, PollenBasicAuthenticateHandler>()
    .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
    .AddSingleton<HttpMessageInvoker, HttpClient>()
    .Configure<AuthenticationOptions>(
        builder.Configuration
            .GetRequiredSection("Authentication")
            .Bind
    );

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IEncryptionService, StubEncryptionService>();

    if (isNcsaCommonLoggingEnabled)
    {
        TextWriter writer =
            isNcsaCommonLoggingEnabled
                ? new StreamWriter("log.txt", true)
                : TextWriter.Null;

        builder.Services.AddNcsaCommonLogging(writer);
    }
}

WebApplication app = builder.Build();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment() && isNcsaCommonLoggingEnabled)
{
    app.UseNcsaCommonLogging();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
return 0;
