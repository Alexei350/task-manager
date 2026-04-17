using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Controllers.Base;
using TaskManager.Service.Interfaces;
using TaskManager.Service.Base;

namespace TaskManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ApiKeyController(IServiceProvider provider) : BaseController(provider)
    {
        private readonly IApiKeyService _service = provider.GetRequiredService<IApiKeyService>();
        private readonly IRequestContext _requestContext = provider.GetRequiredService<IRequestContext>();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiKeyModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Create(_requestContext.UserId, model.Name, model.ExpiresAt);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            });
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.List(_requestContext.UserId);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Revoke(Guid id)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _service.Revoke(_requestContext.UserId, id);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            });
        }
    }

    public class CreateApiKeyModel
    {
        public string Name { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
