using System.Threading.Tasks;
using Ehmini.Application.DTOs.Auth;

namespace Ehmini.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<TokenResponseDto> RefreshAsync(TokenRequestDto dto);
    Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto dto);
    Task<RequestResetPasswordResponseDto> RequestResetPasswordAsync(RequestResetPasswordDto dto);
    Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto);
    Task<object> ConfirmAccountAsync(ConfirmAccountRequestDto dto);
    Task<object> GetConfirmationCodeAsync(GetConfirmationCodeRequestDto dto);
    Task<UpdateUserInfoResponseDto> UpdateUserInfoAsync(UpdateUserInfoRequestDto dto);

}
