using System;
using System.ComponentModel;

namespace TaskManager.Utils.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Busca a descrição de um item de um enum
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum enumObj)
        {
            if (enumObj == null)
                return string.Empty;

            var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            if (fieldInfo == null)
                return string.Empty;

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumObj.ToString();
        }
    }
}