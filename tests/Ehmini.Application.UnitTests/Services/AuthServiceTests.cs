using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.Interfaces;
using Ehmini.Application.Services;
using Ehmini.Application.UnitTests.Helpers;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Ehmini.Application.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICountryRepository> _countryRepoMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = UserManagerMockHelper.MockUserManager<ApplicationUser>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _countryRepoMock = new Mock<ICountryRepository>();

        //_authService = new AuthService(
        //    _userManagerMock.Object,
        //    _tokenServiceMock.Object,
        //    _emailServiceMock.Object,
        //    _unitOfWorkMock.Object,
        //    _countryRepoMock.Object
        //);
    }

    // Méthode helper locale pour simuler proprement la propriété .Users du UserManager
    private void SetupMockUsers(List<ApplicationUser> users)
    {
        // On transforme la liste en notre Enumerable Asynchrone customisé
        var asyncQueryable = new AsyncDbEnumerable<ApplicationUser>(users);

        _userManagerMock.Setup(m => m.Users).Returns(asyncQueryable);
    }

    #region 1. RefreshAsync Tests
    [Fact]
    public async Task RefreshAsync_WithValidTokens_ShouldReturnNewTokens()
    {
        // Arrange
        var dto = new TokenRequestDto { AccessToken = "valid-expired-jwt", RefreshToken = "valid-refresh" };
        var user = new ApplicationUser { Email = "momo@gmail.com", RefreshToken = "valid-refresh", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) };

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }));
        _tokenServiceMock.Setup(t => t.GetPrincipalFromExpiredToken(dto.AccessToken)).Returns(claimsPrincipal);
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        //_tokenServiceMock.Setup(t => t.GenerateJwtToken(user)).Returns("new-jwt");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh");
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RefreshAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-jwt");
        result.RefreshToken.Should().Be("new-refresh");
    }
    #endregion

    #region 2. LoginAsync Tests
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponseDto()
    {
        // Arrange
        var dto = new LoginRequestDto("momo@gmail.com", "Momo1359");
        var user = new ApplicationUser { Email = "momo@gmail.com", UserName = "momo" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        //_tokenServiceMock.Setup(t => t.GenerateJwtToken(user)).Returns("jwt");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh");
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        //result.Token.Should().Be("jwt");
        result.RefreshToken.Should().Be("refresh");
    }
    #endregion

    #region 3. RegisterAsync Tests
    [Fact]
    public async Task RegisterAsync_WithNewUser_ShouldSucceedAndSendEmail()
    {
        // Arrange
        var dto = new RegisterRequestDto(
            Email: "momo@gmail.com",
            Password: "Momo1359",
            Username: "momo",
            FullName: "Mohamed L",
            Cin: "12345678",
            Phone: "555123",
            BirthDate: DateTime.UtcNow.AddYears(-25),
            CountryId: 1,
            AddressId: 1,
            ProfessionId: 1
        );

        SetupMockUsers(new List<ApplicationUser>()); // Liste vide = aucun doublon trouvé

        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "ClientEhmini")).ReturnsAsync(IdentityResult.Success);
        _emailServiceMock.Setup(e => e.SendEmailAsync(dto.Email, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 4. ChangePasswordAsync Tests
    [Fact]
    public async Task ChangePasswordAsync_WithValidOldPassword_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new ChangePasswordRequestDto("momo@gmail.com", "Momo1359", "NewMomo1234");
        var user = new ApplicationUser { Email = "momo@gmail.com" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword)).ReturnsAsync(IdentityResult.Success);
        _emailServiceMock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _authService.ChangePasswordAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 5. RequestResetPasswordAsync Tests
    [Fact]
    public async Task RequestResetPasswordAsync_WithValidEmail_ShouldSendCode()
    {
        // Arrange
        var dto = new RequestResetPasswordDto("momo@gmail.com");
        var user = new ApplicationUser { Email = "momo@gmail.com" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Identifier)).ReturnsAsync(user);
        _emailServiceMock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RequestResetPasswordAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 6. ResetPasswordAsync Tests
    [Fact]
    public async Task ResetPasswordAsync_WithValidToken_ShouldResetSuccessfully()
    {
        // Arrange
        var dto = new ResetPasswordRequestDto("momo@gmail.com", "123456", "NewPassword1359");
        var user = new ApplicationUser { Email = "momo@gmail.com", AccountConfirmationToken = "123456", PwdResetTokenCreationDate = DateTime.UtcNow };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Username)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.RemovePasswordAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddPasswordAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 7. ConfirmAccountAsync Tests
    [Fact]
    public async Task ConfirmAccountAsync_WithCorrectCode_ShouldConfirmAndReturnToken()
    {
        // Arrange
        var dto = new ConfirmAccountRequestDto("momo@gmail.com", "Password1359", "123456");
        var user = new ApplicationUser { Email = "momo@gmail.com", AccountConfirmationToken = "123456", IsAccountConfirmed = false };

        SetupMockUsers(new List<ApplicationUser> { user });

        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        //_tokenServiceMock.Setup(t => t.GenerateJwtToken(user)).Returns("new-jwt-token");

        // Act
        var result = await _authService.ConfirmAccountAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 8. GetConfirmationCodeAsync Tests
    [Fact]
    public async Task GetConfirmationCodeAsync_WhenAccountNotConfirmed_ShouldResendEmail()
    {
        // Arrange
        var dto = new GetConfirmationCodeRequestDto("momo@gmail.com", "Password1359");
        var user = new ApplicationUser { Email = "momo@gmail.com", IsAccountConfirmed = false };

        SetupMockUsers(new List<ApplicationUser> { user });

        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _emailServiceMock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.GetConfirmationCodeAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }
    #endregion

    #region 9. UpdateUserInfoAsync Tests
    [Fact]
    public async Task UpdateUserInfoAsync_WithValidCin_ShouldUpdateFieldsSuccessfully()
    {
        // Arrange
        var dto = new UpdateUserInfoRequestDto(
            Cin: "12345678",
            FullName: "New Full Name",
            Email: "newemail@gmail.com",
            Phone: "555123",
            CountryIsoCode: "TN",
            Signature: "signature-data",
            Birthday: DateTime.UtcNow.AddYears(-25),
            ProfessionId: 6,
            AddressId: 3
        );
        var user = new ApplicationUser { Cin = "12345678", FullName = "Old Name", Email = "old@gmail.com" };
        var country = new Country { Id = 1, IsoCode = "TN" };

        SetupMockUsers(new List<ApplicationUser> { user });

        _countryRepoMock.Setup(c => c.GetByIsoCodeAsync(dto.CountryIsoCode ?? "TN")).ReturnsAsync(country);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.UpdateUserInfoAsync(dto);

        // Assert
        result.Should().NotBeNull();
        user.FullName.Should().Be(dto.FullName);
        user.Email.Should().Be(dto.Email);
        user.CountryId.Should().Be(country.Id);
    }
    #endregion


}
public class AsyncDbProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public AsyncDbProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    // 1. Implémentation de la méthode non générique requise par IQueryProvider
    public IQueryable CreateQuery(Expression expression)
    {
        return new AsyncDbEnumerable<T>(expression);
    }

    // 2. Implémentation de la méthode générique
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new AsyncDbEnumerable<TElement>(expression);
    }

    public object? Execute(Expression expression) => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // EF réécrit AnyAsync avec un type de retour Task<bool> ou bool encapsulé.
        // On extrait le type sous-jacent si TResult est un Task.
        var expectedResultType = typeof(TResult);
        if (expectedResultType.IsGenericType && expectedResultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            expectedResultType = expectedResultType.GetGenericArguments()[0];
        }

        // On exécute l'expression de manière synchrone sur la liste mémoire
        var executionResult = _inner.Execute(expression);

        // Si le type attendu est un Task, on l'encapsule dans un Task.FromResult
        if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
        {
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult })!;
        }

        return (TResult)executionResult!;
    }
}

public class AsyncDbEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public AsyncDbEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public AsyncDbEnumerable(Expression expression) : base(expression) { }

    IQueryProvider IQueryable.Provider => new AsyncDbProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new AsyncDbEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

public class AsyncDbEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public AsyncDbEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
}