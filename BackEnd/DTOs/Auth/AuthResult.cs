namespace BackEnd.DTOs.Auth
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
