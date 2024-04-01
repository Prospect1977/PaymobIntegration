namespace ClassLibrary.Models.Users
{
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
