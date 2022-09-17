using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace DeveloperApiTest.Infraestrutura.Swagger;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensao
{
    public static IServiceCollection AdicionarSwagger(this IServiceCollection services, string apiVersion)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = assemblyName, Version = apiVersion });

            //resolver conflitos
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            var xmlFile = $"{assemblyName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AdicionarSwaggerAutorizacaoJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = string.Empty;
        try
        {
            secretKey = configuration.GetValue<string>("AuthSecretKey");

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentNullException("Por favor verifique as configurações no appsettings pela chave AuthSecretKey");
            }
        }
        catch
        {
            throw;
        }

        var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(configureOptions =>
        {
            configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.ConfigureSwaggerGen(c =>
        {
            var jwtSecuritySchemeDefinition = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition("Bearer", jwtSecuritySchemeDefinition);

            var jwtSecuritySchemeRequirement = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        { jwtSecuritySchemeRequirement, Array.Empty<string>() }
                });
        });

        return services;
    }

    public static void UsarSwaggerUIMultiplasVersoes(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
    }
}
