using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Models;

public partial class ChromeContext : DbContext
{
    public ChromeContext()
    {
    }

    public ChromeContext(DbContextOptions<ChromeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountManagement> AccountManagements { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<GroupFunction> GroupFunctions { get; set; }

    public virtual DbSet<GroupManagement> GroupManagements { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<LocationMaster> LocationMasters { get; set; }

    public virtual DbSet<Movement> Movements { get; set; }

    public virtual DbSet<MovementDetail> MovementDetails { get; set; }

    public virtual DbSet<OrderType> OrderTypes { get; set; }

    public virtual DbSet<PickList> PickLists { get; set; }

    public virtual DbSet<PickListDetail> PickListDetails { get; set; }

    public virtual DbSet<ProductMaster> ProductMasters { get; set; }

    public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }

    public virtual DbSet<PutAway> PutAways { get; set; }

    public virtual DbSet<PutAwayDetail> PutAwayDetails { get; set; }

    public virtual DbSet<PutAwayRule> PutAwayRules { get; set; }

    public virtual DbSet<StatusMaster> StatusMasters { get; set; }

    public virtual DbSet<StockIn> StockIns { get; set; }

    public virtual DbSet<StockInDetail> StockInDetails { get; set; }

    public virtual DbSet<StockOut> StockOuts { get; set; }

    public virtual DbSet<StockOutDetail> StockOutDetails { get; set; }

    public virtual DbSet<StockTake> StockTakes { get; set; }

    public virtual DbSet<StockTakeDetail> StockTakeDetails { get; set; }

    public virtual DbSet<StorageCategory> StorageCategories { get; set; }

    public virtual DbSet<StorageCategoryProduct> StorageCategoryProducts { get; set; }

    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    public virtual DbSet<TransferDetail> TransferDetails { get; set; }

    public virtual DbSet<WarehouseMaster> WarehouseMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Chrome;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountManagement>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F28457B7FA5A08");

            entity.ToTable("AccountManagement");

            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Group).WithMany(p => p.AccountManagements)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__AccountMa__Group__571DF1D5");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BE1EE1B7C");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__06678520E0C29F82");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(12);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF918C0343C16");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId }).HasName("PK__GroupFun__87804C9B4DDBD228");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Function).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Funct__5165187F");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Group__5070F446");
        });

        modelBuilder.Entity<GroupManagement>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30A1013BC12");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseCode, e.LocationCode, e.ProductCode, e.LotNo }).HasName("PK__Inventor__4806EC15AAD2C77C");

            entity.ToTable("Inventory");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__70DDC3D8");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__71D1E811");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__6FE99F9F");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationCode).HasName("PK__Location__DDB144D4FB258946");

            entity.ToTable("LocationMaster");

            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.StorageCategoryId)
                .HasMaxLength(100)
                .HasColumnName("StorageCategoryID");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.StorageCategory).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.StorageCategoryId)
                .HasConstraintName("FK__LocationM__Stora__6D0D32F4");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__LocationM__Wareh__6C190EBB");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.MovementCode).HasName("PK__Movement__8B71BE1C148D1D7E");

            entity.ToTable("Movement");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.FromLocation).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToLocation).HasMaxLength(100);
            entity.Property(e => e.TransferDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.FromLocationNavigation).WithMany(p => p.MovementFromLocationNavigations)
                .HasForeignKey(d => d.FromLocation)
                .HasConstraintName("FK__Movement__FromLo__40058253");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Movement__OrderT__3D2915A8");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Movement__Respon__40F9A68C");

            entity.HasOne(d => d.Status).WithMany(p => p.Movements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Movement__Status__41EDCAC5");

            entity.HasOne(d => d.ToLocationNavigation).WithMany(p => p.MovementToLocationNavigations)
                .HasForeignKey(d => d.ToLocation)
                .HasConstraintName("FK__Movement__ToLoca__3F115E1A");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Movement__Wareho__3E1D39E1");
        });

        modelBuilder.Entity<MovementDetail>(entity =>
        {
            entity.HasKey(e => new { e.MovementCode, e.ProductCode }).HasName("PK__Movement__79855E3867AC870E");

            entity.ToTable("MovementDetail");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.MovementCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.MovementCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Movem__44CA3770");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Produ__45BE5BA9");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeCode).HasName("PK__OrderTyp__6FE0625548925464");

            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.PickNo).HasName("PK__PickList__C80F569CF3E85A9B");

            entity.ToTable("PickList");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PickDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PickList__Locati__208CD6FA");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PickList__OrderT__1F98B2C1");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PickList__Respon__2180FB33");

            entity.HasOne(d => d.Status).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PickList__Status__22751F6C");
        });

        modelBuilder.Entity<PickListDetail>(entity =>
        {
            entity.HasKey(e => new { e.PickNo, e.ProductCode }).HasName("PK__PickList__3AFBB6B8FB5F19BC");

            entity.ToTable("PickListDetail");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.PickNoNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.PickNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__PickN__25518C17");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__Produ__2645B050");
        });

        modelBuilder.Entity<ProductMaster>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__ProductM__2F4E024E81D06B3E");

            entity.ToTable("ProductMaster");

            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.BaseUom)
                .HasMaxLength(50)
                .HasColumnName("BaseUOM");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Uom)
                .HasMaxLength(50)
                .HasColumnName("UOM");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductMasters)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__ProductMa__Categ__5DCAEF64");
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.SupplierCode, e.ProductCode }).HasName("PK__ProductS__B64A783E42EE00C5");

            entity.ToTable("ProductSupplier");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Produ__619B8048");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.SupplierCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Suppl__60A75C0F");
        });

        modelBuilder.Entity<PutAway>(entity =>
        {
            entity.HasKey(e => e.PutAwayCode).HasName("PK__PutAway__4283C165BB1F49AF");

            entity.ToTable("PutAway");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PutAwayDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAway__Locatio__3493CFA7");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PutAway__OrderTy__339FAB6E");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PutAway__Respons__3587F3E0");

            entity.HasOne(d => d.Status).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PutAway__StatusI__367C1819");
        });

        modelBuilder.Entity<PutAwayDetail>(entity =>
        {
            entity.HasKey(e => new { e.PutAwayCode, e.ProductCode }).HasName("PK__PutAwayD__B07721411CD7DA1D");

            entity.ToTable("PutAwayDetail");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__Produ__3A4CA8FD");

            entity.HasOne(d => d.PutAwayCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.PutAwayCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__PutAw__395884C4");
        });

        modelBuilder.Entity<PutAwayRule>(entity =>
        {
            entity.HasKey(e => e.PutAwayRuleCode).HasName("PK__PutAwayR__6FBD724A9F27A2F2");

            entity.Property(e => e.PutAwayRuleCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.StorageCategoryId)
                .HasMaxLength(100)
                .HasColumnName("StorageCategoryID");
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            entity.Property(e => e.WarehouseToApply).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAwayRu__Locat__76969D2E");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__PutAwayRu__Produ__75A278F5");

            entity.HasOne(d => d.StorageCategory).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.StorageCategoryId)
                .HasConstraintName("FK__PutAwayRu__Stora__778AC167");

            entity.HasOne(d => d.WarehouseToApplyNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.WarehouseToApply)
                .HasConstraintName("FK__PutAwayRu__Wareh__74AE54BC");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE2043CE122B58");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.HasKey(e => e.StockInCode).HasName("PK__StockIn__3FAEE5C53A7928E7");

            entity.ToTable("StockIn");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.OrderDeadline).HasColumnType("datetime");
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockIn__OrderTy__7E37BEF6");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockIn__Respons__01142BA1");

            entity.HasOne(d => d.Status).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockIn__StatusI__02084FDA");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.SupplierCode)
                .HasConstraintName("FK__StockIn__Supplie__00200768");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockIn__Warehou__7F2BE32F");
        });

        modelBuilder.Entity<StockInDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockInCode, e.ProductCode }).HasName("PK__StockInD__CD5A05E163C08575");

            entity.ToTable("StockInDetail");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Produ__05D8E0BE");

            entity.HasOne(d => d.StockInCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.StockInCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Stock__04E4BC85");
        });

        modelBuilder.Entity<StockOut>(entity =>
        {
            entity.HasKey(e => e.StockOutCode).HasName("PK__StockOut__C9B8C8566C6E5BF1");

            entity.ToTable("StockOut");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StockOutDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.CustomerCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.CustomerCode)
                .HasConstraintName("FK__StockOut__Custom__17036CC0");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockOut__OrderT__151B244E");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockOut__Respon__17F790F9");

            entity.HasOne(d => d.Status).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockOut__Status__18EBB532");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockOut__Wareho__160F4887");
        });

        modelBuilder.Entity<StockOutDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockOutCode, e.ProductCode }).HasName("PK__StockOut__3B4C287213769883");

            entity.ToTable("StockOutDetail");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Produ__1CBC4616");

            entity.HasOne(d => d.StockOutCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.StockOutCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Stock__1BC821DD");
        });

        modelBuilder.Entity<StockTake>(entity =>
        {
            entity.HasKey(e => e.StockTakeCode).HasName("PK__StockTak__365EA42A2505E68B");

            entity.ToTable("StockTake");

            entity.Property(e => e.StockTakeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TransferDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockTakes)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockTake__Respo__498EEC8D");

            entity.HasOne(d => d.Status).WithMany(p => p.StockTakes)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockTake__Statu__4A8310C6");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockTakes)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockTake__Wareh__489AC854");
        });

        modelBuilder.Entity<StockTakeDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockTakeCode, e.ProductCode, e.LocationCode, e.LotNo }).HasName("PK__StockTak__5D03E347A54388D1");

            entity.ToTable("StockTakeDetail");

            entity.Property(e => e.StockTakeCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.StockTakeDetails)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTake__Locat__4D5F7D71");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockTakeDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTake__Produ__4F47C5E3");

            entity.HasOne(d => d.StockTakeCodeNavigation).WithMany(p => p.StockTakeDetails)
                .HasForeignKey(d => d.StockTakeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTake__Stock__4E53A1AA");
        });

        modelBuilder.Entity<StorageCategory>(entity =>
        {
            entity.HasKey(e => e.StorageCategoryId).HasName("PK__StorageC__F6A70FE9520C1A13");

            entity.ToTable("StorageCategory");

            entity.Property(e => e.StorageCategoryId)
                .HasMaxLength(100)
                .HasColumnName("StorageCategoryID");
        });

        modelBuilder.Entity<StorageCategoryProduct>(entity =>
        {
            entity.HasKey(e => new { e.StorageCategoryId, e.ProductCode }).HasName("PK__StorageC__0453EFCDBB655ABD");

            entity.ToTable("StorageCategoryProduct");

            entity.Property(e => e.StorageCategoryId)
                .HasMaxLength(100)
                .HasColumnName("StorageCategoryID");
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StorageCategoryProducts)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StorageCa__Produ__6754599E");

            entity.HasOne(d => d.StorageCategory).WithMany(p => p.StorageCategoryProducts)
                .HasForeignKey(d => d.StorageCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StorageCa__Stora__66603565");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SupplierCode).HasName("PK__Supplier__44BE981AF3D7D406");

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.SupplierPhone).HasMaxLength(12);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.TransferCode).HasName("PK__Transfer__CE99A4C44FD81611");

            entity.ToTable("Transfer");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.FromWarehouseCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToWarehouseCode).HasMaxLength(100);
            entity.Property(e => e.TransferDate).HasColumnType("datetime");

            entity.HasOne(d => d.FromWarehouseCodeNavigation).WithMany(p => p.TransferFromWarehouseCodeNavigations)
                .HasForeignKey(d => d.FromWarehouseCode)
                .HasConstraintName("FK__Transfer__FromWa__2A164134");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Transfer__OrderT__29221CFB");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Transfer__Respon__2BFE89A6");

            entity.HasOne(d => d.Status).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Transfer__Status__2CF2ADDF");

            entity.HasOne(d => d.ToWarehouseCodeNavigation).WithMany(p => p.TransferToWarehouseCodeNavigations)
                .HasForeignKey(d => d.ToWarehouseCode)
                .HasConstraintName("FK__Transfer__ToWare__2B0A656D");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransferCode, e.ProductCode }).HasName("PK__Transfer__3C6D44E0213B88DA");

            entity.ToTable("TransferDetail");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Produ__30C33EC3");

            entity.HasOne(d => d.TransferCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Trans__2FCF1A8A");
        });

        modelBuilder.Entity<WarehouseMaster>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("PK__Warehous__1686A057C38C36B9");

            entity.ToTable("WarehouseMaster");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
