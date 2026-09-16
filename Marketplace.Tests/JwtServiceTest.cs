
using Marketplace.Application.Services;
using Marketplace.Domain.Interfaces;
using Marketplace.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Marketplace.Tests
{
    public class JwtServiceTest
    {
        private readonly JwtService _service;

        private const string JwtKey =
            "MySuperSecretJwtKey12345678901234567890";

        public JwtServiceTest()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = "Marketplace",
                ["Jwt:Audience"] = "MarketplaceUsers"
            };

            IConfiguration configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(settings)
                    .Build();

            _service = new JwtService(configuration);
        }

        [Fact]
        public void GenerateRefreshToken_Should_ReturnNonEmptyToken()
        {
            var token = _service.GenerateRefreshToken();
            
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateRefreshToken_Should_GenerateDifferentTokens()
        {
            var token1 = _service.GenerateRefreshToken();
            var token2 = _service.GenerateRefreshToken();

            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateRefreshToken_Should_HaveExpectedBase64Length()
        {
            var token = _service.GenerateRefreshToken();

            // 32 bytes converted to Base64 = 44 chars
            Assert.Equal(44, token.Length);
        }

        [Fact]
        public void GenerateToken_Should_CreateValidJwt()
        {
            var user = new ApplicationUser
            {
                Id = "123",
                UserName = "Mahmoud",
                Email = "mahmoud@gmail.com"
            };

            var roles = new List<string> { "User", "Seller" };

            var token = _service.GenerateToken(user, roles);

            Assert.NotNull(token);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("123",
                jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            Assert.Equal("Mahmoud",
                jwt.Claims.First(x => x.Type == ClaimTypes.Name).Value);

            Assert.Equal("mahmoud@gmail.com",
                jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value);
            
            var roleClaims = jwt.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();

            Assert.Contains("User", roleClaims);
            Assert.Contains("Seller", roleClaims);
        }

        [Fact]
        public void GenerateToken_Should_CreateTokenThatCanBeValidated()
        {
            var user = new ApplicationUser
            {
                Id = "123",
                UserName = "Mahmoud",
                Email = "mahmoud@example.com"
            };

            var token = _service.GenerateToken(
                user,
                new List<string> { "User" });

            var result = _service.ValidateToken(token);

            Assert.True(result);
        }

        [Fact]
        public void ValidateToken_Should_ReturnFalse_ForInvalidToken()
        {
            var result =
                _service.ValidateToken("invalid-token");

            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_Should_ReturnFalse_WhenTokenIsModified()
        {
            var user = new ApplicationUser
            {
                Id = "123",
                UserName = "Mahmoud",
                Email = "mahmoud@example.com"
            };

            var token = _service.GenerateToken(
                user,
                new List<string> { "User" });

            var modifiedToken = token + "modified";

            var result =
                _service.ValidateToken(modifiedToken);

            Assert.False(result);
        }
    }
}
