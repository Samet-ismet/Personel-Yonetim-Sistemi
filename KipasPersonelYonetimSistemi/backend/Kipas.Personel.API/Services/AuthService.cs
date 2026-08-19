using Microsoft.AspNetCore.Identity;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IRefreshTokenRepository
            _refreshTokenRepository;
        private readonly IPasswordHasher<AppUser>
            _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAppUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher<AppUser> passwordHasher,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }

        
        public async Task<AuthResponseDto?> LoginAsync(
            LoginDto dto)
        {
            var normalizedUsername =
                dto.Username.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByUsernameAsync(
                    normalizedUsername);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning(
                    "Başarısız giriş denemesi. Kullanıcı adı: {Username}",
                    normalizedUsername);

                return null;
            }

            var verificationResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);

            if (verificationResult ==
                PasswordVerificationResult.Failed)
            {
                _logger.LogWarning(
                    "Başarısız giriş denemesi. Kullanıcı Id: {UserId}",
                    user.Id);

                return null;
            }

            if (verificationResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(
                        user,
                        dto.Password);

                await _userRepository.SaveChangesAsync();
            }

            var response =
                await CreateSessionAsync(user);

            _logger.LogInformation(
                "Kullanıcı giriş yaptı. Kullanıcı Id: {UserId}",
                user.Id);

            return response;
        }

        public async Task<AuthResponseDto?> RefreshAsync(
            string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var tokenHash =
                _tokenService.HashRefreshToken(
                    refreshToken);

            var storedToken =
                await _refreshTokenRepository
                    .GetByHashAsync(tokenHash);

            if (storedToken == null ||
                !storedToken.IsActive)
            {
                _logger.LogWarning(
                    "Geçersiz veya süresi dolmuş refresh token kullanıldı.");

                return null;
            }


            if (!storedToken.AppUser.IsActive)
            {
                storedToken.RevokedAt = DateTime.UtcNow;

                await _refreshTokenRepository
                    .SaveChangesAsync();

                _logger.LogWarning(
                    "Pasif kullanıcı refresh token kullanmaya çalıştı. Kullanıcı Id: {UserId}",
                    storedToken.AppUserId);

                return null;
            }

            var now = DateTime.UtcNow;

            var newRefreshToken =
                _tokenService.CreateRefreshToken();

            var newRefreshTokenHash =
                _tokenService.HashRefreshToken(
                    newRefreshToken);

            var refreshExpiration =
                now.AddDays(
                    GetRefreshTokenExpirationDays());

            storedToken.RevokedAt = now;
            storedToken.ReplacedByTokenHash =
                newRefreshTokenHash;

            var replacementToken =
                new RefreshToken
                {
                    TokenHash = newRefreshTokenHash,
                    CreatedAt = now,
                    ExpiresAt = refreshExpiration,
                    AppUserId = storedToken.AppUserId
                };

            await _refreshTokenRepository.AddAsync(
                replacementToken);

            await _refreshTokenRepository
                .SaveChangesAsync();

            var accessToken =
                _tokenService.CreateAccessToken(
                    storedToken.AppUser,
                    out var accessTokenExpiration);

            _logger.LogInformation(
                "Refresh token yenilendi. Kullanıcı Id: {UserId}",
                storedToken.AppUserId);

            return new AuthResponseDto
            {
                Token = accessToken,
                Expiration = accessTokenExpiration,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration =
                    refreshExpiration,
                Username = storedToken.AppUser.Username,
                Role = storedToken.AppUser.Role
            };
        }

        public async Task LogoutAsync(
            string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var tokenHash =
                _tokenService.HashRefreshToken(
                    refreshToken);

            var storedToken =
                await _refreshTokenRepository
                    .GetByHashAsync(tokenHash);

            if (storedToken == null ||
                !storedToken.IsActive)
            {
                return;
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository
                .SaveChangesAsync();

            _logger.LogInformation(
                "Kullanıcı oturumu sonlandırıldı. Kullanıcı Id: {UserId}",
                storedToken.AppUserId);
        }

        private async Task<AuthResponseDto>
            CreateSessionAsync(AppUser user)
        {
            var accessToken =
                _tokenService.CreateAccessToken(
                    user,
                    out var accessTokenExpiration);

            var refreshToken =
                _tokenService.CreateRefreshToken();

            var tokenHash =
                _tokenService.HashRefreshToken(
                    refreshToken);

            var refreshTokenExpiration =
                DateTime.UtcNow.AddDays(
                    GetRefreshTokenExpirationDays());

            var refreshTokenEntity =
                new RefreshToken
                {
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = refreshTokenExpiration,
                    AppUserId = user.Id
                };

            await _refreshTokenRepository.AddAsync(
                refreshTokenEntity);

            await _refreshTokenRepository
                .SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = accessToken,
                Expiration = accessTokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpiration =
                    refreshTokenExpiration,
                Username = user.Username,
                Role = user.Role
            };
        }

        private int GetRefreshTokenExpirationDays()
        {
            if (!int.TryParse(
                    _configuration[
                        "Jwt:RefreshTokenExpirationDays"],
                    out var expirationDays) ||
                expirationDays <= 0)
            {
                throw new InvalidOperationException(
                    "Refresh token geçerlilik süresi geçerli bir pozitif sayı olmalıdır.");
            }

            return expirationDays;
        }
    }
}