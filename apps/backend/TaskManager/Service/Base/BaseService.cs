using System;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Service.Interfaces.Base;
using TaskManager.Utils.i18n;

namespace TaskManager.Service.Base
{
    public abstract class BaseService(IServiceProvider provider) : IBaseService
    {
        protected readonly IResourceStringLocalizer Localizer = provider.GetService<IResourceStringLocalizer>();
        protected readonly IServiceProvider Provider = provider;
        protected readonly IRequestContext Context = provider.GetService<IRequestContext>();
    }
}