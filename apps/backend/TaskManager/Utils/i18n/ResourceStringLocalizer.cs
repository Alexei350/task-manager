using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace TaskManager.Utils.i18n
{
    public interface IResourceStringLocalizer
    {
        /// <summary>
        /// Busca a tradução conforme a chave
        /// </summary>
        /// <param name="key">Chave do texto a ser traduzido</param>
        /// <returns></returns>
        string GetString(string key);
    }

    public class ResourceStringLocalizer : IResourceStringLocalizer
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResourceManager _resourceManager;

        public ResourceStringLocalizer(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;

            SetCulture();
            
            // Inicializa o ResourceManager com a cultura já definida
            _resourceManager = new ResourceManager("TaskManager.Resources.i18n.General", typeof(ResourceStringLocalizer).Assembly);
        }

        /// <summary>
        /// Busca a tradução de acordo com a chave
        /// </summary>
        /// <param name="key">Chave do texto a ser traduzido</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            EnsureCultureIsSet();
            return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        }

        /// <summary>
        /// Garante que a cultura está definida antes de buscar strings
        /// </summary>
        private void EnsureCultureIsSet()
        {
            // Se a cultura atual for invariante, tenta definir novamente
            if (CultureInfo.CurrentUICulture.Equals(CultureInfo.InvariantCulture))
            {
                SetCulture();
            }
        }

        /// <summary>
        /// Define a linguagem a ser utilizada para as traduções
        /// </summary>
        /// <returns></returns>
        private void SetCulture()
        {
            CultureInfo defaultCulture = new(_config["DefaultCulture"] ?? string.Empty);
            CultureInfo userCulture = default;

            try
            {
                var requestCulture = _httpContextAccessor
                    .HttpContext
                    ?.Request
                    .GetTypedHeaders()
                    .AcceptLanguage
                    .FirstOrDefault()
                    ?.Value
                    .Value;

                if (requestCulture != null)
                    userCulture = new CultureInfo(requestCulture);
            }
            catch
            {
                // ignored
            }

            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = userCulture ?? defaultCulture;
        }
    }
}