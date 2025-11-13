namespace GadgetHubAPI.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int CustomerId { get; set; }
    }
}