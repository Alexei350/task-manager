using FakeItEasy;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Models.Request.User;
using TaskManager.Repository;
using TaskManager.Service;
using TaskManager.Service.Base;
using TaskManager.Utils.i18n;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.UnitTests.Service;

public class UserServiceTests
{
    private readonly UserService _service;
    private readonly IResourceStringLocalizer _localizer;
    private readonly IUserRepository _userRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRequestContext _requestContext;

    public UserServiceTests()
    {
        //Dependências
        _localizer = A.Fake<IResourceStringLocalizer>();
        _userRepository = A.Fake<IUserRepository>();
        _requestContext = A.Fake<IRequestContext>();
        _serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer)))
            .Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(IUserRepository)))
            .Returns(_userRepository);
        A.CallTo(() => _serviceProvider.GetService(typeof(IRequestContext)))
            .Returns(_requestContext);

        //SUT
        _service = new UserService(_serviceProvider);
    }

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnSuccess()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "test@mail.com",
            Password = "password123"
        };

        var successMessage = "Criado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenCreating))
            .Returns(successMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(model.Email))
            .Returns(Task.FromResult<User>(null!));
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(successMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Create_WithNullModel_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(null);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithEmptyPassword_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "test@mail.com",
            Password = ""
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithEmptyName_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "",
            Email = "test@mail.com",
            Password = "password123"
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithEmptyEmail_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "",
            Password = "password123"
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithInvalidEmail_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "invalid-email",
            Password = "password123"
        };

        var errorMessage = "Email inválido";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.InvalidEmail))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithDuplicatedEmail_ShouldReturnWarning()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "test@mail.com",
            Password = "password123"
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            Name = "Existing User"
        };

        var warningMessage = "Email já cadastrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.DuplicatedUserEmail))
            .Returns(warningMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(model.Email))
            .Returns(existingUser);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(warningMessage, response.Messages[0].Message);
        Assert.Equal(ReturnMessageTypeEnum.Warning, response.Messages[0].Type);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustNotHaveHappened();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WhenSaveFails_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "test@mail.com",
            Password = "password123"
        };

        var errorMessage = "Erro ao criar";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.ErrorWhenCreating))
            .Returns(errorMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(model.Email))
            .Returns(Task.FromResult<User>(null!));
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(false);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Create_WithGoogleUserId_ShouldCreateUserWithoutPassword()
    {
        //Parametrização
        var model = new CreateUserModel
        {
            Name = "Test User",
            Email = "test@mail.com",
            GoogleUserId = "google-id-123"
        };

        var successMessage = "Criado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenCreating))
            .Returns(successMessage);
        A.CallTo(() => _userRepository.GetByEmailAsync(model.Email))
            .Returns(Task.FromResult<User>(null!));
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);

        A.CallTo(() => _userRepository.CreateAsync(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    #endregion
}
