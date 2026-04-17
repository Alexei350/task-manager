using TaskManager.Utils.Security;

namespace TaskManager.UnitTests.Utils.Security;

public class HashingTests
{
    [Fact]
    public void HashPassword_ShouldReturnHashAndSalt()
    {
        var password = "password123";
        var (hash, salt) = Hashing.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotNull(salt);
        Assert.NotEmpty(hash);
        Assert.NotEmpty(salt);
    }

    [Fact]
    public void ValidatePassword_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        var password = "password123";
        var (hash, salt) = Hashing.HashPassword(password);

        var isValid = Hashing.ValidatePassword(password, hash, salt);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidatePassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        var password = "password123";
        var (hash, salt) = Hashing.HashPassword(password);

        var isValid = Hashing.ValidatePassword("wrongpassword", hash, salt);

        Assert.False(isValid);
    }
}
