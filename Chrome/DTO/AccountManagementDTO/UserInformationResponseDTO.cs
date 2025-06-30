using Chrome.DTO.GroupFunctionDTO;

namespace Chrome.DTO.AccountManagementDTO
{
    public class UserInformationResponseDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string GroupID { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public List<ApplicableLocationResponseDTO> ApplicableLocations { get; set; } = new List<ApplicableLocationResponseDTO>();
    }
}
