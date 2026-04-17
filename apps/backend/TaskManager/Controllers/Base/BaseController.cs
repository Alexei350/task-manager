using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.Models.Base;
using TaskManager.Utils.i18n;

namespace TaskManager.Controllers.Base;

public class BaseController(IServiceProvider provider) : ControllerBase
{
    private readonly IResourceStringLocalizer _localizer = provider.GetService<IResourceStringLocalizer>();
    private readonly ILogger<BaseController> _logger = provider.GetService<ILogger<BaseController>>();
    
    protected async Task<IActionResult> DefaultHandlerAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            return BadRequest(ReturnData.ReturnError(_localizer.GetString(LocalizationDictionary.UnexpectedError)));
        }
    }
    
    protected IActionResult DefaultHandler(Func<IActionResult> action)
    {
        try
        {
            return action();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            return BadRequest(ReturnData.ReturnError(_localizer.GetString(LocalizationDictionary.UnexpectedError)));
        }
    }
}