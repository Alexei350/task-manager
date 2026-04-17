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

public class MeServiceTests
{
    private readonly MeService _service;
    private readonly IResourceStringLocalizer _localizer;
    private readonly IUserRepository _userRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRequestContext _requestContext;

    public MeServiceTests()
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
        _service = new MeService(_serviceProvider);
    }

    #region Get Tests

    [Fact]
    public void Get_WithAuthenticatedUser_ShouldReturnUserData()
    {
        //Parametrização
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@mail.com",
            Role = UserRoleEnum.Default
        };

        A.CallTo(() => _requestContext.User).Returns(user);
        A.CallTo(() => _requestContext.UserId).Returns(userId);

        //Ação
        var response = _service.Get();

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(user.Id, response.Data.Id);
        Assert.Equal(user.Name, response.Data.Name);
        Assert.Equal(user.Email, response.Data.Email);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ShouldReturnSuccess()
    {
        //Parametrização
        var userId = Guid.NewGuid();
        var model = new UpdateUserModel
        {
            Name = "Updated Name"
        };

        var user = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "test@mail.com",
            Deleted = false
        };

        var successMessage = "Atualizado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenUpdating))
            .Returns(successMessage);
        A.CallTo(() => _requestContext.UserId).Returns(userId);
        A.CallTo(() => _userRepository.GetForUpdateAsync(userId))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(successMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>.That.Matches(u => u.Name == model.Name)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Update_WithNullModel_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Update(null);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>._)).MustNotHaveHappened();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithEmptyName_ShouldReturnError()
    {
        //Parametrização
        var model = new UpdateUserModel
        {
            Name = ""
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithNonExistentUser_ShouldReturnError()
    {
        //Parametrização
        var userId = Guid.NewGuid();
        var model = new UpdateUserModel
        {
            Name = "Updated Name"
        };

        var errorMessage = "Registro não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.RecordNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _requestContext.UserId).Returns(userId);
        A.CallTo(() => _userRepository.GetForUpdateAsync(userId))
            .Returns(Task.FromResult<User>(null!));

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>._)).MustNotHaveHappened();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithDeletedUser_ShouldReturnError()
    {
        //Parametrização
        var userId = Guid.NewGuid();
        var model = new UpdateUserModel
        {
            Name = "Updated Name"
        };

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@mail.com",
            Deleted = true
        };

        var errorMessage = "Registro está deletado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.RecordIsDeleted))
            .Returns(errorMessage);
        A.CallTo(() => _requestContext.UserId).Returns(userId);
        A.CallTo(() => _userRepository.GetForUpdateAsync(userId))
            .Returns(user);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>._)).MustNotHaveHappened();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WhenSaveFails_ShouldReturnError()
    {
        //Parametrização
        var userId = Guid.NewGuid();
        var model = new UpdateUserModel
        {
            Name = "Updated Name"
        };

        var user = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "test@mail.com",
            Deleted = false
        };

        var errorMessage = "Erro ao atualizar";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.ErrorWhenUpdating))
            .Returns(errorMessage);
        A.CallTo(() => _requestContext.UserId).Returns(userId);
        A.CallTo(() => _userRepository.GetForUpdateAsync(userId))
            .Returns(user);
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .Returns(false);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _userRepository.Update(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    #endregion
}
