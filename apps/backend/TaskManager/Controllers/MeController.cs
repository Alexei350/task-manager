using System;
using System.Threading.Tasks;
using TaskManager.Service.Interfaces;
using TaskManager.Models.Base;
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
    public class MeController(IServiceProvider provider) : BaseController(provider)
    {
        private readonly IMeService _service = provider.GetService<IMeService>();

        /// <summary>
        /// Retorna os dados do usuário autenticado
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return DefaultHandler(() =>
            {
                ReturnData response = _service.Get();

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
        
        /// <summary>
        /// Edita os dados do usuário autenticado
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Update(model);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
    }
}
