using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Kipas.Personel.API.OpenApi
{
    public sealed class ApiDocumentTransformer
        : IOpenApiDocumentTransformer
    {
        private readonly IAuthenticationSchemeProvider
            _authenticationSchemeProvider;

        public ApiDocumentTransformer(
            IAuthenticationSchemeProvider
                authenticationSchemeProvider)
        {
            _authenticationSchemeProvider =
                authenticationSchemeProvider;
        }

        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Info = new OpenApiInfo
            {
                Title = "Kipaş Personel Yönetim Sistemi API",
                Version = "v1",
                Description =
                    "Personel, kullanıcı, yetkilendirme ve oturum yönetimi işlemlerini sağlayan REST API."
            };

            var authenticationSchemes =
                await _authenticationSchemeProvider
                    .GetAllSchemesAsync();

            var hasBearerScheme =
                authenticationSchemes.Any(
                    scheme =>
                        scheme.Name ==
                        JwtBearerDefaults
                            .AuthenticationScheme);

            if (!hasBearerScheme)
            {
                return;
            }

            document.Components ??=
                new OpenApiComponents();

            document.Components.SecuritySchemes ??=
     new Dictionary<string, OpenApiSecurityScheme>();

            document.Components.SecuritySchemes[
                JwtBearerDefaults.AuthenticationScheme] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Giriş işleminden alınan access tokenı girin. Bearer kelimesini ayrıca yazmayın."
                };
        }
    }

    public sealed class AuthOperationTransformer
        : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var endpointMetadata =
                context.Description
                    .ActionDescriptor
                    .EndpointMetadata;

            var allowsAnonymous =
                endpointMetadata
                    .OfType<IAllowAnonymous>()
                    .Any();

            if (allowsAnonymous)
            {
                return Task.CompletedTask;
            }

            var authorizationData =
                endpointMetadata
                    .OfType<IAuthorizeData>()
                    .ToList();

            if (authorizationData.Count == 0)
            {
                return Task.CompletedTask;
            }

            operation.Security ??=
                new List<OpenApiSecurityRequirement>();

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType
                                            .SecurityScheme,

                                    Id =
                                        JwtBearerDefaults
                                            .AuthenticationScheme
                                }
                        }
                    ] = Array.Empty<string>()
                });

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add(
                    "401",
                    new OpenApiResponse
                    {
                        Description =
                            "Geçerli access token bulunamadı."
                    });
            }

            var requiresRole =
                authorizationData.Any(
                    authorization =>
                        !string.IsNullOrWhiteSpace(
                            authorization.Roles));

            if (requiresRole &&
                !operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add(
                    "403",
                    new OpenApiResponse
                    {
                        Description =
                            "Kullanıcının bu işlem için gerekli rolü bulunmuyor."
                    });
            }

            return Task.CompletedTask;
        }
    }
}