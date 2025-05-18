using Microsoft.AspNetCore.Authorization;

namespace Chrome.Permision
{
    /*
    *  PermissionHandler sẽ so sánh danh sách quyền của user với API đang gọi.
    *  Nếu user có quyền truy cập API đó, request sẽ được phép tiếp tục.
    *  Nếu không có quyền, request sẽ bị chặn.
    */
    public class PermissionHandler: AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //Lấy danh sách quyền từ JWT của user
            var userPermissions = context.User.Claims
                .Where(c=>c.Type=="Permission")
                .Select(c=>c.Value)
                .ToList();

            //Lấy thông tin API mà user đang gọi
            var httpContext = context.Resource as DefaultHttpContext;
            if (httpContext == null)
            {
                return Task.CompletedTask;
            }
            
            //lấy url của api
            var requestedPath = httpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(requestedPath))
            {
                return Task.CompletedTask;
            }

            if (userPermissions.Any(permission=> PermissionToApiPrefixMap.TryGetValue(permission, out var apiPrefix )
                && requestedPath.StartsWith(apiPrefix)))
            {
                context.Succeed(requirement);
            } 
            return Task.CompletedTask;
                
        }

        private static readonly Dictionary<string, string> PermissionToApiPrefixMap = new Dictionary<string, string>()
        {
            {"ucAccountManagement","/api/AccountManagement"},
            {"ucGroupManagement","/api/GroupManagement" },
            {"ucWarehouseMaster","/api/WarehouseMaster" },
            {"ucStockIn","/api/StockIn"},
            {"ucStockOut","/api/StockOut"},
            {"ucProductMaster","/api/ProductMaster"},
            {"ucSupplierMaster","/api/SupplierMaster" },
            {"ucCustomerMaster","/api/CustomerMaster" },
            {"ucInventory","/api/Inventory" },
            {"ucProductStructure","/api/ProductStructure" },
            {"ucPickList","/api/PickList" },
            {"ucTransfer","/api/Transfer" },
            {"ucProductStructureVersion","/api/ProductStructureVersion" },
            {"ucProductionOrder","/api/ProductionOrder" }
        };
    }
}
