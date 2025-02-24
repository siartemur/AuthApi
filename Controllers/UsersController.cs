using AuthApi.Features.Users.Commands.RegisterUser;
using AuthApi.Features.Users.Commands.LoginUser;
using AuthApi.Features.Users.Commands.UpdateUser;
using AuthApi.Features.Users.Commands.DeleteUser;
using AuthApi.Features.Users.Queries.GetUserById;
using AuthApi.Features.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Kullanıcı Kaydı
        /// </summary>
        [Authorize(Roles = "Manager")] // Sadece Manager Kullanıcılar Yeni Kullanıcı Ekleyebilir
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }


        /// <summary>
        /// Kullanıcı Girişi (JWT Token Döndürür)
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        /// <summary>
        /// Kullanıcı Güncelleme
        /// </summary>
        [Authorize(Roles = "Manager,TeamLeader")] // Sadece Manager ve TeamLeader Güncelleme Yapabilir
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return response.Success ? Ok(response) : NotFound(response);
        }


        /// <summary>
        /// Kullanıcı Silme
        /// </summary>
        [Authorize(Roles = "Manager")] // Sadece Manager Kullanıcıları Silebilir
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var response = await _mediator.Send(new DeleteUserCommandRequest { UserId = userId });
            return response.Success ? Ok(response) : NotFound(response);
        }


        /// <summary>
        /// ID'ye Göre Kullanıcı Getirme
        /// </summary>
        [Authorize] // Sadece yetkilendirilmiş kullanıcılar erişebilir
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await _mediator.Send(new GetUserByIdQueryRequest { UserId = userId });
            return response != null ? Ok(response) : NotFound(new { message = "User not found" });
        }

        /// <summary>
        /// Tüm Kullanıcıları Getirme
        /// </summary>
        [Authorize] // Sadece yetkilendirilmiş kullanıcılar erişebilir
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _mediator.Send(new GetAllUsersQueryRequest());
            return Ok(response);
        }
    }
}
