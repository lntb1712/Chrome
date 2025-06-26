using Chrome.Permision;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Chrome.Permission
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissions = context.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var userWarehouses = context.User.Claims
                .Where(c => c.Type == "Warehouse")
                .Select(c => c.Value)
                .ToList();

            var httpContext = context.Resource as DefaultHttpContext;
            if (httpContext == null)
                return Task.CompletedTask;

            var requestedPath = httpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(requestedPath))
                return Task.CompletedTask;

            var warehouseId = httpContext.Request.Query["warehouseId"].ToString();

            if (string.IsNullOrEmpty(warehouseId))
            {
                if (userPermissions.Any(permission =>
                    PermissionToApiPatternMap.TryGetValue(permission, out var apiPattern)
                    && Regex.IsMatch(requestedPath, apiPattern, RegexOptions.IgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail(); // chặn request nếu không match permission
                }

                return Task.CompletedTask;
            }

            if (userPermissions.Any(permission =>
                PermissionToApiPatternMap.TryGetValue(permission, out var apiPattern)
                && Regex.IsMatch(requestedPath, apiPattern, RegexOptions.IgnoreCase))
                && userWarehouses.Contains(warehouseId))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(); // chặn request nếu không match permission
            }

            return Task.CompletedTask;
        }

        private static readonly Dictionary<string, string> PermissionToApiPatternMap = new Dictionary<string, string>()
        {
            {"ucAccountManagement", @"^/api/AccountManagement"},
            {"ucGroupManagement", @"^/api/GroupManagement"},
            {"ucWarehouseMaster", @"^/api/WarehouseMaster"},
            {"ucStockIn", @"^/api/StockIn"},
            {"ucStockOut", @"^/api/StockOut"},
            {"ucProductMaster", @"^/api/ProductMaster"},
            {"ucSupplierMaster", @"^/api/SupplierMaster"},
            {"ucCustomerMaster", @"^/api/CustomerMaster"},
            {"ucInventory", @"^/api/Inventory"},
            {"ucBOMMaster", @"^/api/BOMMaster"},
            {"ucPickList", @"^/api/PickList"},
            {"ucTransfer", @"^/api/Transfer"},
            {"ucProductStructureVersion", @"^/api/ProductStructureVersion"},
            {"ucManufacturingOrder", @"^/api/ManufacturingOrder"},
            {"ucProductSupplier", @"^/api/ProductMaster/[^/]+/ProductSupplier"},
            {"ucProductCustomer", @"^/api/ProductMaster/[^/]+/ProductCustomer"},
            {"ucStorageProduct", @"^/api/StorageProduct"},
            {"ucPutAwayRules",@"^/api/PutAwayRules" },
            {"ucReservation",@"^/api/Reservation" },
            {"ucPutAway",@"^/api/PutAway" },
            {"ucMovement",@"^/api/Movement" },
            {"ucStockTake",@"^/api/StockTake" }
        };
    }
}
