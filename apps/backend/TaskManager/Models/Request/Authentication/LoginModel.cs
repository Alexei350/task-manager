namespace TaskManager.Models.Request.Authentication
{
    public class LoginModel
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Hash da senha
        /// </summary>
        public string Password { get; set; }
    }
}