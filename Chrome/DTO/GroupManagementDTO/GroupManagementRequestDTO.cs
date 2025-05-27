using Chrome.DTO.GroupFunctionDTO;
namespace Chrome.DTO.GroupManagementDTO
{
    public class GroupManagementRequestDTO
    {

        public string GroupId { get; set; } = null!;
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public string? UpdateBy { get; set; }
        public List<GroupFunctionResponseDTO> GroupFunctions { get; set; } = new List<GroupFunctionResponseDTO>();
    }
}
