﻿using Chrome.Permision;
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

                return Task.CompletedTask;
            }

            if (userPermissions.Any(permission =>
                PermissionToApiPatternMap.TryGetValue(permission, out var apiPattern)
                && Regex.IsMatch(requestedPath, apiPattern, RegexOptions.IgnoreCase))
                && userWarehouses.Contains(warehouseId))
            {
                context.Succeed(requirement);
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
            {"ucProductStructure", @"^/api/ProductStructure"},
            {"ucPickList", @"^/api/PickList"},
            {"ucTransfer", @"^/api/Transfer"},
            {"ucProductStructureVersion", @"^/api/ProductStructureVersion"},
            {"ucProductionOrder", @"^/api/ProductionOrder"},
            {"ucProductSupplier", @"^/api/ProductMaster/[^/]+/ProductSupplier"},
            {"ucProductCustomer", @"^/api/ProductMaster/[^/]+/ProductCustomer"}
        };
    }
}
