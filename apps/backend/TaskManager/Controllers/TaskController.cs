using System;
using System.Threading.Tasks;
using TaskManager.Service.Interfaces;
using TaskManager.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Controllers.Base;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Request.Base;

namespace TaskManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class TaskController(IServiceProvider provider) : BaseController(provider)
    {
        private readonly ITaskService _service = provider.GetService<ITaskService>();

        /// <summary>
        /// Adiciona uma tarefa
        /// </summary>
        /// <param name="model">Dados do tarefa</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Create(model);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
        
        /// <summary>
        /// Adiciona uma tarefa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateTaskModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Update(model);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
        
        /// <summary>
        /// Adiciona uma tarefa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Delete(id);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
        
        /// <summary>
        /// Retorna uma tarefa pelo Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return await DefaultHandlerAsync(async () =>
            {
                ReturnData response = await _service.Get(id);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }

        /// <summary>
        /// Busca uma lista paginada de tarefas
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetPagedModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                ReturnData response = await _service.GetPaged(model);

                if (!response.Success)
                    return BadRequest(response);
                
                return Ok(response);
            });
        }
    }
}
