using System.Collections.Generic;

namespace AuthApi.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryResponse
    {
        public List<UserDto> Users { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
