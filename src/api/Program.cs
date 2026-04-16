using System;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Shipstone.AspNetCore.Http;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Weather;
using Shipstone.Pollen.Api.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

bool isNcsaCommonLoggingEnabled =
    builder.Configuration.GetValue<bool>("IsNcsaCommonLoggingEnabled");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        IConfigurationSection authentication =
            builder.Configuration.GetRequiredSection("Authentication");

        String? signingKey = authentication["SigningKey"];

        if (signingKey is null)
        {
            throw new InvalidOperationException("The provided configuration does not contain a signing key.");
        }

        byte[] bytes = Convert.FromBase64String(signingKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidAudience = authentication["Audience"],
            ValidIssuer = authentication["Issuer"],
            ValidateIssuerSigningKey = true
        };
    });

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
    .AddPollenCore()
    .AddPollenInfrastructureData()
    .AddPollenInfrastructureWeather(
        builder.Configuration
            .GetRequiredSection("Weather")
            .Bind
    )
    .AddSingleton<HttpMessageInvoker, HttpClient>();

if (isNcsaCommonLoggingEnabled)
{
    TextWriter writer = new StreamWriter("log.txt", true);
    builder.Services.AddNcsaCommonLogging(writer);
}

WebApplication app = builder.Build();
app.UseHttpsRedirection();

if (isNcsaCommonLoggingEnabled)
{
    app.UseNcsaCommonLogging();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
return 0;
