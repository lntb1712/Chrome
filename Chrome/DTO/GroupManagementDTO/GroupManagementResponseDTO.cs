namespace Chrome.DTO.GroupManagementDTO
{
    public class GroupManagementResponseDTO
    {
        public string GroupId { get; set; } = null!;
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateTime { get; set; }
    }
}
