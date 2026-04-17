using System;
using System.Threading.Tasks;
using TaskManager.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Controllers.Base;
using TaskManager.Models.Request.User;

namespace TaskManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController(IServiceProvider provider) : BaseController(provider)
    {
        private readonly IUserService _service = provider.GetService<IUserService>();

        /// <summary>
        /// Adiciona um usuário
        /// </summary>
        /// <param name="model">Dados do usuário</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Create(model);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
    }
}
