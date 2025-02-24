using AuthApi.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthApi.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQueryRequest, GetAllUsersQueryResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetAllUsersQueryResponse> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();

            return new GetAllUsersQueryResponse
            {
                Users = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email
                }).ToList()
            };
        }
    }
}
