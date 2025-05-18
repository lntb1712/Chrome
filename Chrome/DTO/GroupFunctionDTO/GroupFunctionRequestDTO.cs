
namespace Chrome.DTO.GroupFunctionDTO
{
    public class GroupFunctionRequestDTO
    {
        public string GroupId { get; set; } = null!;
        public string FunctionId { get; set; } = null!;
        public bool? IsEnable { get; set; }
        public string? UpdateBy { get; set; }
        
    }
}
