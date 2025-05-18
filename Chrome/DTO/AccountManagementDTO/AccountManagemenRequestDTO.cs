namespace Chrome.DTO.AccountManagementDTO
{
    public class AccountManagementRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string GroupID { get; set; } = string.Empty;
        public string UpdateBy { get; set; } = string.Empty;
    }
}
