namespace TaskManager.Models.Request.User
{
    public class CreateUserModel
    {
        /// <summary>
        /// Hash da senha
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } 

        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id (sub) do usuário no Google
        /// </summary>
        public string GoogleUserId { get; set; }
    }
}