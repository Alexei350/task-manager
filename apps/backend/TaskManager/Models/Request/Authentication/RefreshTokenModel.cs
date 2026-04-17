namespace TaskManager.Models.Request.Authentication
{
    public class RefreshTokenModel
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}