using Chrome.Models;
using Chrome.Permision;
using Chrome.Permission;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.BOMComponentRepository;
using Chrome.Repositories.BOMMasterRepository;
using Chrome.Repositories.CategoryRepository;
using Chrome.Repositories.CustomerMasterRepository;
using Chrome.Repositories.FunctionRepository;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Repositories.GroupManagementRepository;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.LocationMasterRepository;
using Chrome.Repositories.ProductCustomerRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.ProductSupplierRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.StorageProductRepository;
using Chrome.Repositories.SupplierMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Chrome.Services.AccountManagementService;
using Chrome.Services.BOMComponentService;
using Chrome.Services.BOMMasterService;
using Chrome.Services.CategoryService;
using Chrome.Services.CustomerMasterService;
using Chrome.Services.FunctionService;
using Chrome.Services.GroupFunctionService;
using Chrome.Services.GroupManagementService;
using Chrome.Services.InventoryService;
using Chrome.Services.JWTService;
using Chrome.Services.LocationMasterService;
using Chrome.Services.LoginService;
using Chrome.Services.ProductCustomerService;
using Chrome.Services.ProductMasterService;
using Chrome.Services.ProductSupplierSerivce;
using Chrome.Services.PutAwayRulesService;
using Chrome.Services.StorageProductService;
using Chrome.Services.SupplierMasterService;
using Chrome.Services.WarehouseMasterService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductionInventoryManagmentSystem_API.Repositories.GroupFunctionRepository;
using ProductionInventoryManagmentSystem_API.Services.GroupManagementService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DBContext
builder.Services.AddDbContext<ChromeContext>
    (option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
// Add services to the container.

// Đăng kí Dependency Injection cho các Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IGroupFunctionRepository, GroupFunctionRepository>();
builder.Services.AddScoped<IGroupManagementRepository,GroupManagementRepository>();
builder.Services.AddScoped<IFunctionRepository, FunctionRepository>();
builder.Services.AddScoped<IProductMasterRepository, ProductMasterRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductSupplierRepository, ProductSupplierRepository>();
builder.Services.AddScoped<ISupplierMasterRepository, SupplierMasterRepository>();
builder.Services.AddScoped<ICustomerMasterRepository, CustomerMasterRepository>();
builder.Services.AddScoped<IProductCustomerRepository, ProductCustomerRepository>();
builder.Services.AddScoped<IWarehouseMasterRepository, WarehouseMasterRepository>();
builder.Services.AddScoped<ILocationMasterRepository, LocationMasterRepository>();
builder.Services.AddScoped<IStorageProductRepository, StorageProductRepository>();
builder.Services.AddScoped<IPutAwayRulesRepository, PutAwayRulesRepository>();
builder.Services.AddScoped<IBOMMasterRepository, BOMMasterRepository>();
builder.Services.AddScoped<IBOMComponentRepository,BOMComponentRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();


// Đăng kí Dependency Injection cho các Service
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IGroupManagementService, GroupManagementService>();
builder.Services.AddScoped<IGroupFunctionService, GroupFunctionService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IProductMasterService, ProductMasterService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductSupplierService, ProductSupplierService>();
builder.Services.AddScoped<ISupplierMasterService, SupplierMasterService>();
builder.Services.AddScoped<ICustomerMasterService, CustomerMasterService>();
builder.Services.AddScoped<IProductCustomerService, ProductCustomerService>();
builder.Services.AddScoped<IWarehouseMasterService, WarehouseMasterService>();
builder.Services.AddScoped<ILocationMasterService, LocationMasterService>();
builder.Services.AddScoped<IStorageProductService, StorageProductService>();
builder.Services.AddScoped<IPutAwayRulesService, PutAwayRulesService>();
builder.Services.AddScoped<IBOMMasterService, BOMMasterService>();
builder.Services.AddScoped<IBOMComponentService,BOMComponentService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
// Đăng ký IHttpContextAccessor để thực hiện sử dụng HttpCookie
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // bỏ qua các giá trị null khi api trả về kết quả
    }); // câu lệnh này có công dụng giữ nguyên tên thuộc tính được định nghĩa trong class C# (nhớ phải cài newtonsoftJson)

// Cấu hình Authentication JWT
var secretKey = builder.Configuration["AppSettings:SecretKey"];


// Map chuỗi SecretKey thành mảng byte để dùng thuật toán xét đối xứng
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

// Add authentication JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false; // Phát triển trên HTTP
        option.SaveToken = true;
        option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            // Ký vào token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // dùng thuật toán SymmetricSecurityKey để kiểm tra đối xứng mảng secretKeyBytes

            ClockSkew = TimeSpan.Zero // Để giảm độ trễ khi kiểm tra token hết hạn

            //// Bảo mật hơn
            //ValidateIssuer = true, // Kiểm tra đúng API cấp token
            //ValidIssuer = issuer,

            //ValidateAudience = true, // Kiểm tra đúng frontend gửi request 
            //ValidAudience = audience,

            //// Ký vào token
            //ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // dùng thuật toán SymmetricSecurityKey để kiểm tra đối xứng mảng secretKeyBytes

            //ValidateLifetime = true, // Kiểm tra hạn sử dụng token
            //ClockSkew = TimeSpan.Zero // Giảm độ trễ khi kiểm tra token hết hạn

        };
    });


// Add authorization JWT Bearer
//Thêm một policy duy nhất để kiểm tra quyền dựa trên claim "permissions":
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PermissionPolicy", policy =>
    {
        policy.Requirements.Add(new PermissionRequirement());
    });
});

/*
 * Singleton nghĩa là gì?
 * ASP.NET Core sẽ chỉ tạo một instance duy nhất của PermissionHandler trong suốt vòng đời của ứng dụng.
 * Tất cả các request đều dùng chung một instance này.
 * Giúp tối ưu hiệu suất và giảm chi phí tạo mới nhiều lần.
 */

builder.Services.AddSingleton<IAuthorizationHandler,PermissionHandler>();

builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // ✅ BỔ SUNG THÔNG TIN PHIÊN BẢN
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Chrome API",
        Description = "API documentation for Chrome"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey, // MUST be ApiKey for Swagger 2.0
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None));
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

// sử dụng authen trước author
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
