using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Chrome.Permision
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public PermissionRequirement() { }
    }
}
