using Microsoft.AspNetCore.Mvc;
using Models.Users;
using Models.Orders;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService usersService;
        private readonly SessionsService sessionsService;

        public UsersController(UsersService usersService, SessionsService sessionsService)
        {
            this.usersService = usersService;
            this.sessionsService = sessionsService;
        }

        [HttpPost("register")]
        public async Task<int> Register(User user)
        {
            var userId = await this.usersService.Register(user);
            await this.sessionsService.RegisterSession(userId);
            return userId;
        }

        [HttpPost("login")]
        public async Task<int> Login(User user)
        {
            var userId = await this.usersService.Login(user);
            await this.sessionsService.StartSession(userId);
            return userId;
        }

        [HttpPost("logout/{id}")]
        public async Task Logout(int userId)
        {
            await this.sessionsService.EndSession(userId);
        }

        [HttpPatch("addProduct")]
        public async Task AddProductToShoppingCart(OrderDetail orderDetail)
        {
            await this.sessionsService.AddProductToShoppingCart(orderDetail, this.sessionsService.SessionId);
        }

        [HttpPatch("removeProduct")]
        public async Task RemoveProductFromShoppingCart(OrderDetail orderDetail)
        {
            await this.sessionsService.RemoveProductFromShoppingCart(orderDetail, this.sessionsService.SessionId);
        }
    }
}
