using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;
using TaskManager.Models.Base;
using TaskManager.Models.Enums;
using TaskManager.Repository;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;

namespace TaskManager.Service
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IResourceStringLocalizer _localizer;

        public ApiKeyService(IApiKeyRepository repository, IUserRepository userRepository, IResourceStringLocalizer localizer)
        {
            _repository = repository;
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task<ReturnData<string>> Create(Guid userId, string name, DateTime? expiresAt = null)
        {
            var user = await _userRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return ReturnData<string>.ReturnError(_localizer.GetString(LocalizationDictionary.UserNotFound));

            // Generate Key
            var keyBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            var randomString = Convert.ToBase64String(keyBytes).Replace("+", "").Replace("/", "").Replace("=", "");
            var prefix = "tm_sk_";
            var apiKey = $"{prefix}{randomString}";

            // Hash Key
            var keyHash = HashKey(apiKey);

            var entity = new ApiKey
            {
                UserId = userId,
                Name = name,
                Prefix = prefix + randomString.Substring(0, 4) + "...",
                KeyHash = keyHash,
                ExpiresAt = expiresAt
            };

            await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();

            return new ReturnData<string>
            {
                Success = true,
                Messages = [ new ReturnMessage(ReturnMessageTypeEnum.Success, _localizer.GetString(LocalizationDictionary.ApiKeyCreated)) ],
                Data = apiKey
            };
        }

        public async Task<ReturnData<List<ApiKey>>> List(Guid userId)
        {
            var keys = await _repository.Query().AsNoTracking()
                .Where(x => x.UserId == userId && !x.Deleted)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            // Don't return the hash
            foreach (var key in keys)
            {
                key.KeyHash = null;
            }

            return new ReturnData<List<ApiKey>>
            {
                Success = true,
                Messages = [ new ReturnMessage(ReturnMessageTypeEnum.Success, _localizer.GetString(LocalizationDictionary.ApiKeyList)) ],
                Data = keys
            };
        }

        public async Task<ReturnData> Revoke(Guid userId, Guid keyId)
        {
            var key = await _repository.Query().FirstOrDefaultAsync(x => x.Id == keyId && x.UserId == userId);
            if (key == null)
                return ReturnData.ReturnError(_localizer.GetString(LocalizationDictionary.RecordNotFound));

            key.Deleted = true;
            key.DeletedAt = DateTime.UtcNow;
            
            _repository.Update(key);
            await _repository.SaveChangesAsync();

            return ReturnData.ReturnSuccess(_localizer.GetString(LocalizationDictionary.ApiKeyRevoked));
        }

        public async Task<ReturnData<User>> Validate(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return ReturnData<User>.ReturnError(_localizer.GetString(LocalizationDictionary.ApiKeyInvalid));

            var keyHash = HashKey(apiKey);

            var key = await _repository.Query()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.KeyHash == keyHash && !x.Deleted);

            if (key == null)
                return ReturnData<User>.ReturnError(_localizer.GetString(LocalizationDictionary.ApiKeyInvalid));

            if (key.ExpiresAt.HasValue && key.ExpiresAt.Value < DateTime.UtcNow)
                return ReturnData<User>.ReturnError(_localizer.GetString(LocalizationDictionary.ApiKeyExpired));

            // Update LastUsed
            key.LastUsedAt = DateTime.UtcNow;
            _repository.Update(key);
            await _repository.SaveChangesAsync();

            return new ReturnData<User>
            {
                Success = true,
                Messages = [ new ReturnMessage(ReturnMessageTypeEnum.Success, _localizer.GetString(LocalizationDictionary.ApiKeyValid)) ],
                Data = key.User
            };
        }

        private string HashKey(string key)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
