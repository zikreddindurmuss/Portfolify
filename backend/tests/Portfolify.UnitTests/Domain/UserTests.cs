using Portfolify.Domain.Entities;
using Xunit;

namespace Portfolify.UnitTests.Domain;

// TODO (Ders 2): Testler implement edilecek
// Örnek test senaryoları:
//   - User.Create() geçerli parametrelerle User döndürmeli
//   - User.Create() email'i lowercase yapmalı
//   - User.Create() slug'ı lowercase yapmalı
//   - User.ChangePassword() UpdatedAt'i güncellemeli

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsUser()
    {
        // Arrange
        const string email = "Test@Example.com";
        const string username = "testuser";
        const string slug = "Test-User";
        const string passwordHash = "$2a$11$hash";

        // Act
        var user = User.Create(email, username, slug, passwordHash);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("test-user", user.Slug);
        Assert.Equal(passwordHash, user.PasswordHash);
    }
}
