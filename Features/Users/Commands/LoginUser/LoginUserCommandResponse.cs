namespace AuthApi.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
