using MediatR;
using System.Collections.Generic;

namespace AuthApi.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<GetAllUsersQueryResponse> { }
}
