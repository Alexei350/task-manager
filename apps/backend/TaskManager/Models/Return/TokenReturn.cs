namespace TaskManager.Models.Return
{
    public class TokenReturn
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
        
        public UserReturn User { get; set; }
    }
}