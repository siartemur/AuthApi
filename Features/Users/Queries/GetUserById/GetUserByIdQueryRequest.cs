using MediatR;

namespace AuthApi.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryRequest : IRequest<GetUserByIdQueryResponse>
    {
        public string UserId { get; set; }
    }
}
