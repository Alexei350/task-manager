using FakeItEasy;
using Microsoft.Extensions.Configuration;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Models.Request.Authentication;
using TaskManager.Repository;
using TaskManager.Service;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;
using TaskManager.Utils.Security;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.UnitTests.Service;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _service;
    private readonly IConfiguration _configuration;
    private readonly IResourceStringLocalizer _localizer;
    private readonly IUserRepository _userRepository;
    private readonly IApiKeyService _apiKeyService;
    
    public AuthenticationServiceTests()
    {
        //Dependências
        _configuration = A.Fake<IConfiguration>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _userRepository = A.Fake<IUserRepository>();
        _apiKeyService = A.Fake<IApiKeyService>();
        
        //Configuração do JWT (usar chave de teste, nunca usar credenciais de produção)
        A.CallTo(() => _configuration["Jwt:Key"]).Returns("test-jwt-key-min-32-chars-for-hs256");
        A.CallTo(() => _configuration["Jwt:Issuer"]).Returns("admin");
        A.CallTo(() => _configuration["Jwt:Audience"]).Returns("task-manager");
        A.CallTo(() => _configuration["GoogleClientId"]).Returns("test-client-id");
        
        //SUT
        _service = new AuthenticationService(_configuration, _localizer, _userRepository, _apiKeyService);
    }
    
    #region GenerateToken Tests
    
    [Fact]
    public async Task GenerateToken_WithValidCredentials_ShouldReturnSuccess()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Name = "Test User",
            Password = hash,
            Salt = salt,
            Role = UserRoleEnum.Default,
            Deleted = false
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.False(string.IsNullOrEmpty(response.Data.Token));
        Assert.False(string.IsNullOrEmpty(response.Data.RefreshToken));
        Assert.NotNull(response.Data.User);
        Assert.Equal(user.Email, response.Data.User.Email);
        Assert.Equal(user.Name, response.Data.User.Name);
        
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task GenerateToken_WithNullModel_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);
        
        //Ação
        var response = await _service.GenerateToken(null);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
        
        A.CallTo(() => _userRepository.GetByEmailAsync(A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task GenerateToken_WithEmptyEmail_ShouldReturnError()
    {
        //Parametrização
        var loginModel = new LoginModel
        {
            Email = "",
            Password = "password123"
        };
        
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task GenerateToken_WithEmptyPassword_ShouldReturnError()
    {
        //Parametrização
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = ""
        };
        
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task GenerateToken_WithNonExistentUser_ShouldReturnError()
    {
        //Parametrização
        var loginModel = new LoginModel
        {
            Email = "notfound@mail.com",
            Password = "password123"
        };
        
        var errorMessage = "Usuário não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(System.Threading.Tasks.Task.FromResult<User>(null!));
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task GenerateToken_WithDeletedUser_ShouldReturnError()
    {
        //Parametrização
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Deleted = true
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        var errorMessage = "Usuário não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task GenerateToken_WithIncorrectPassword_ShouldReturnError()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("correctPassword");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Password = hash,
            Salt = salt,
            Deleted = false
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "wrongPassword"
        };
        
        var errorMessage = "Senha incorreta";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncorrectPassword))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        
        //Ação
        var response = await _service.GenerateToken(loginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    #endregion
    
    #region RefreshToken Tests
    
    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnSuccess()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Name = "Test User",
            Password = hash,
            Salt = salt,
            Role = UserRoleEnum.Default,
            RefreshToken = "valid-refresh-token",
            Deleted = false
        };
        
        // Primeiro gera um token válido
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        A.CallTo(() => _userRepository.GetForUpdateAsync(user.Id))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);
        
        var tokenResponse = await _service.GenerateToken(loginModel);
        
        // Agora testa o refresh
        var refreshModel = new RefreshTokenModel
        {
            Token = tokenResponse.Data.Token,
            RefreshToken = tokenResponse.Data.RefreshToken
        };
        
        user.RefreshToken = tokenResponse.Data.RefreshToken;
        
        //Ação
        var response = await _service.RefreshToken(refreshModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.False(string.IsNullOrEmpty(response.Data.Token));
        Assert.False(string.IsNullOrEmpty(response.Data.RefreshToken));
        Assert.NotEqual(tokenResponse.Data.Token, response.Data.Token);
    }
    
    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnError()
    {
        //Parametrização
        var refreshModel = new RefreshTokenModel
        {
            Token = "invalid-token",
            RefreshToken = "some-refresh-token"
        };
        
        var errorMessage = "Erro ao validar token";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.TokenValidationError))
            .Returns(errorMessage);
        
        //Ação
        var response = await _service.RefreshToken(refreshModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
    }
    
    [Fact]
    public async Task RefreshToken_WithNonExistentUser_ShouldReturnError()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Name = "Test User",
            Password = hash,
            Salt = salt,
            Role = UserRoleEnum.Default,
            Deleted = false
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);
        
        var tokenResponse = await _service.GenerateToken(loginModel);
        
        var refreshModel = new RefreshTokenModel
        {
            Token = tokenResponse.Data.Token,
            RefreshToken = tokenResponse.Data.RefreshToken
        };
        
        var errorMessage = "Usuário não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetForUpdateAsync(user.Id))
            .Returns(System.Threading.Tasks.Task.FromResult<User>(null!));
        
        //Ação
        var response = await _service.RefreshToken(refreshModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task RefreshToken_WithDeletedUser_ShouldReturnError()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Name = "Test User",
            Password = hash,
            Salt = salt,
            Role = UserRoleEnum.Default,
            Deleted = false
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);
        
        var tokenResponse = await _service.GenerateToken(loginModel);
        
        var refreshModel = new RefreshTokenModel
        {
            Token = tokenResponse.Data.Token,
            RefreshToken = tokenResponse.Data.RefreshToken
        };
        
        user.Deleted = true;
        
        var errorMessage = "Usuário não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetForUpdateAsync(user.Id))
            .Returns(user);
        
        //Ação
        var response = await _service.RefreshToken(refreshModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    [Fact]
    public async Task RefreshToken_WithMismatchedRefreshToken_ShouldReturnError()
    {
        //Parametrização
        var (hash, salt) = Hashing.HashPassword("password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@mail.com",
            Name = "Test User",
            Password = hash,
            Salt = salt,
            Role = UserRoleEnum.Default,
            RefreshToken = "stored-refresh-token",
            Deleted = false
        };
        
        var loginModel = new LoginModel
        {
            Email = "test@mail.com",
            Password = "password123"
        };
        
        A.CallTo(() => _userRepository.GetByEmailAsync(loginModel.Email))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);
        
        var tokenResponse = await _service.GenerateToken(loginModel);
        
        var refreshModel = new RefreshTokenModel
        {
            Token = tokenResponse.Data.Token,
            RefreshToken = "different-refresh-token"
        };
        
        var errorMessage = "Refresh token inválido";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.InvalidRefreshToken))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetForUpdateAsync(user.Id))
            .Returns(user);
        
        //Ação
        var response = await _service.RefreshToken(refreshModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }
    
    #endregion
    
    #region GoogleLogin Tests
    
    [Fact]
    public async Task GoogleLogin_WithInvalidToken_ShouldReturnError()
    {
        //Parametrização
        var googleLoginModel = new GoogleLoginModel
        {
            Token = "invalid-google-token"
        };
        
        var errorMessage = "Token inválido";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.InvalidToken))
            .Returns(errorMessage);
        
        //Ação
        var response = await _service.GoogleLogin(googleLoginModel);
        
        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
    }
    
    #endregion
}
