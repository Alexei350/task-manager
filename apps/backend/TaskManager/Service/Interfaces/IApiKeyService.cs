using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Models.Entities;
using TaskManager.Models.Base;

namespace TaskManager.Service.Interfaces
{
    public interface IApiKeyService
    {
        Task<ReturnData<string>> Create(Guid userId, string name, DateTime? expiresAt = null);
        Task<ReturnData> Revoke(Guid userId, Guid keyId);
        Task<ReturnData<List<ApiKey>>> List(Guid userId);
        Task<ReturnData<User>> Validate(string apiKey);
    }
}
