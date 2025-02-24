namespace AuthApi.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? UserName { get; internal set; }
    }
}
