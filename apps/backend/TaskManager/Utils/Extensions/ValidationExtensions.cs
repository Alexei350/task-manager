using System.Text.RegularExpressions;

namespace TaskManager.Utils.Extensions
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Valida se a string é um CPF válido
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidCpf(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            input = input.OnlyNumbers();
            if (input.Length != 11)
                return false;

            for (var j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == input)
                    return false;

            var tempCpf = input[..9];
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return input.EndsWith(digito);
        }

        /// <summary>
        /// Verifica se a string é um CNPJ válido
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidCnpj(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            int[] multiplicador1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            input = input.OnlyNumbers();
            if (input.Length != 14)
                return false;

            var tempCnpj = input[..12];
            var soma = 0;

            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;
            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return input.EndsWith(digito);
        }

        /// <summary>
        /// Valida um número de telefone/celular
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidPhone(this string input) => PhoneRegex().IsMatch(input);

        /// <summary>
        /// Valida um CEP 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidZipCode(this string input) => ZipCodeRegex().IsMatch(input);

        /// <summary>
        /// Valida um e-mail
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
                
            return EmailRegex().IsMatch(input);
        }

        /// <summary>
        /// Regex para números de telefone/celular (exige o DDD, não aceita DDI)
        /// </summary>
        /// <returns></returns>
        private static Regex PhoneRegex() => new(@"^(?:\(?([1-9][0-9])\)?\s?)(?:((?:9\d|[2-9])\d{3})\-?(\d{4}))$", RegexOptions.Compiled);

        /// <summary>
        /// Regex para validação de CEP
        /// </summary>
        /// <returns></returns>
        private static Regex ZipCodeRegex() => new(@"^\d{5}-\d{3}$", RegexOptions.Compiled);

        /// <summary>
        /// Regex para validação de e-mail
        /// </summary>
        /// <returns></returns>
        private static Regex EmailRegex() => new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    }
}