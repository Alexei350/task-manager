using System.Text.RegularExpressions;
using TaskManager.Models.Enums;

namespace TaskManager.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Retorna uma string somente com números
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string OnlyNumbers(this string input) => NotNumberRegex().Replace(input, string.Empty);

        /// <summary>
        /// Formata um CPF/CNPJ
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FormatTxId(this string txId, TxIdTypeEnum type)
        {
            var treatedTxId = txId.OnlyNumbers();

            switch (type)
            {
                case TxIdTypeEnum.Cpf:
                    if (treatedTxId.Length != 11)
                        return treatedTxId;
                    
                    return treatedTxId
                        .Insert(3, ".")
                        .Insert(7, ".")
                        .Insert(11, "-");

                case TxIdTypeEnum.Cnpj:
                    if (treatedTxId.Length != 14)
                        return treatedTxId;

                    return treatedTxId
                        .Insert(2, ".")
                        .Insert(6, ".")
                        .Insert(10, "/")
                        .Insert(15, "-");
                    
                default:
                    return treatedTxId;
            }
        }

        /// <summary>
        /// Aplica a máscara no número de telefone
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatPhone(this string input)
        {
            var phone = input.OnlyNumbers();

            if (phone.Length < 10)
                return phone;

            return phone
                .Insert(phone.Length - 4, "-")
                .Insert(2, ") ")
                .Insert(0, "(");
        } 

        /// <summary>
        /// Aplica a máscara do CEP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatZipCode(this string input)
        {
            var zipCode = input.OnlyNumbers();

            return zipCode.Length != 8 
                ? zipCode 
                : zipCode.Insert(5, "-");
        }
        
        public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

        /// <summary>
        /// Regex para encontrar caracteres que não são numéricos
        /// </summary>
        /// <returns></returns>
        private static Regex NotNumberRegex() => new("[^0-9]+", RegexOptions.Compiled);
    }
}