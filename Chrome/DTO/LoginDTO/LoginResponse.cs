namespace Chrome.DTO.LoginDTO
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? GroupId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
