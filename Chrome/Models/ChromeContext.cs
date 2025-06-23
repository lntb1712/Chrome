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

    public virtual DbSet<BomComponent> BomComponents { get; set; }

    public virtual DbSet<BomOperation> BomOperations { get; set; }

    public virtual DbSet<Bommaster> Bommasters { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }

    public virtual DbSet<CustomerProduct> CustomerProducts { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<GroupFunction> GroupFunctions { get; set; }

    public virtual DbSet<GroupManagement> GroupManagements { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<LocationMaster> LocationMasters { get; set; }

    public virtual DbSet<ManufWorkOrder> ManufWorkOrders { get; set; }

    public virtual DbSet<ManufactoringOrderDetail> ManufactoringOrderDetails { get; set; }

    public virtual DbSet<ManufacturingOrder> ManufacturingOrders { get; set; }

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

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationDetail> ReservationDetails { get; set; }

    public virtual DbSet<StatusMaster> StatusMasters { get; set; }

    public virtual DbSet<StockIn> StockIns { get; set; }

    public virtual DbSet<StockInDetail> StockInDetails { get; set; }

    public virtual DbSet<StockOut> StockOuts { get; set; }

    public virtual DbSet<StockOutDetail> StockOutDetails { get; set; }

    public virtual DbSet<Stocktake> Stocktakes { get; set; }

    public virtual DbSet<StocktakeDetail> StocktakeDetails { get; set; }

    public virtual DbSet<StorageProduct> StorageProducts { get; set; }

    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    public virtual DbSet<TransferDetail> TransferDetails { get; set; }

    public virtual DbSet<WarehouseMaster> WarehouseMasters { get; set; }

    public virtual DbSet<WorkCenterMaster> WorkCenterMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Chrome;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountManagement>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F2845771F53CB0");

            entity.ToTable("AccountManagement");

            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");

            entity.HasOne(d => d.Group).WithMany(p => p.AccountManagements)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__AccountMa__Group__4BAC3F29");
        });

        modelBuilder.Entity<BomComponent>(entity =>
        {
            entity.HasKey(e => new { e.Bomcode, e.ComponentCode, e.BomVersion }).HasName("PK__BOM_Comp__568017D4E7C1917E");

            entity.ToTable("BOM_Component");

            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.ComponentCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.BomComponents)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Compo__Compo__2EDAF651");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.BomComponents)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Component__2DE6D218");
        });

        modelBuilder.Entity<BomOperation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Bom_Operation");

            entity.Property(e => e.BomCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);
            entity.Property(e => e.OperationCode).HasMaxLength(100);
            entity.Property(e => e.WorkCenterCode).HasMaxLength(100);
        });

        modelBuilder.Entity<Bommaster>(entity =>
        {
            entity.HasKey(e => new { e.Bomcode, e.Bomversion }).HasName("PK__BOMMaste__1DABFE8731EDC856");

            entity.ToTable("BOMMaster");

            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.Bomversion)
                .HasMaxLength(100)
                .HasColumnName("BOMVersion");
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Bommasters)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__BOMMaster__Produ__2B0A656D");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BB4CA21C6");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__06678520894849F4");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<CustomerProduct>(entity =>
        {
            entity.HasKey(e => new { e.CustomerCode, e.ProductCode }).HasName("PK__Customer__F4936504C2E36EE0");

            entity.ToTable("CustomerProduct");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.CustomerCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.CustomerCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Custo__6383C8BA");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Produ__6477ECF3");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF918AE48E7D2");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId, e.ApplicableLocation }).HasName("PK__GroupFun__CAF59E7E9043389C");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
            entity.Property(e => e.ApplicableLocation).HasMaxLength(100);
            entity.Property(e => e.IsEnable).HasColumnName("isEnable");

            entity.HasOne(d => d.ApplicableLocationNavigation).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.ApplicableLocation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Appli__5441852A");

            entity.HasOne(d => d.Function).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Funct__534D60F1");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Group__52593CB8");
        });

        modelBuilder.Entity<GroupManagement>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30A4B808158");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseCode, e.LocationCode, e.ProductCode, e.Lotno }).HasName("PK__Inventor__BB86ECC7A50C230E");

            entity.ToTable("Inventory");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ReceiveDate).HasColumnType("datetime");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__6EF57B66");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__6FE99F9F");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__6E01572D");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationCode).HasName("PK__Location__DDB144D4DCFDE5AE");

            entity.ToTable("LocationMaster");

            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.IsEmpty).HasColumnName("isEmpty");
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__LocationM__Stora__6B24EA82");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__LocationM__Wareh__6A30C649");
        });

        modelBuilder.Entity<ManufWorkOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Manuf_WorkOrder");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.OperationCode).HasMaxLength(100);
            entity.Property(e => e.WorkCenterCode).HasMaxLength(100);
        });

        modelBuilder.Entity<ManufactoringOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.ManufacturingOrderCode, e.ComponentCode }).HasName("PK__Manufact__D558EF7C1E65FF92");

            entity.ToTable("ManufactoringOrderDetail");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.ComponentCode).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.ManufactoringOrderDetails)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufacto__Compo__56E8E7AB");
        });

        modelBuilder.Entity<ManufacturingOrder>(entity =>
        {
            entity.HasKey(e => new { e.ManufacturingOrderCode, e.ProductCode, e.Bomcode }).HasName("PK__Manufact__20712A7CD2A22B7C");

            entity.ToTable("ManufacturingOrder");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.BomVersion).HasMaxLength(100);
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.ScheduleDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.WorkCenterCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufactu__Produ__51300E55");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Manufactu__Respo__531856C7");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .HasConstraintName("FK__ManufacturingOrd__5224328E");

            entity.HasOne(d => d.WorkCenterMaster).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => new { d.WorkCenterCode, d.WarehouseCode })
                .HasConstraintName("FK__ManufacturingOrd__540C7B00");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.MovementCode).HasName("PK__Movement__8B71BE1C715FDE85");

            entity.ToTable("Movement");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.FromLocation).HasMaxLength(100);
            entity.Property(e => e.MovementDate).HasColumnType("datetime");
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToLocation).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.FromLocationNavigation).WithMany(p => p.MovementFromLocationNavigations)
                .HasForeignKey(d => d.FromLocation)
                .HasConstraintName("FK__Movement__FromLo__208CD6FA");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Movement__OrderT__1EA48E88");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Movement__Respon__22751F6C");

            entity.HasOne(d => d.Status).WithMany(p => p.Movements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Movement__Status__236943A5");

            entity.HasOne(d => d.ToLocationNavigation).WithMany(p => p.MovementToLocationNavigations)
                .HasForeignKey(d => d.ToLocation)
                .HasConstraintName("FK__Movement__ToLoca__2180FB33");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Movement__Wareho__1F98B2C1");
        });

        modelBuilder.Entity<MovementDetail>(entity =>
        {
            entity.HasKey(e => new { e.MovementCode, e.ProductCode }).HasName("PK__Movement__79855E38F78C4423");

            entity.ToTable("MovementDetail");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.MovementCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.MovementCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Movem__2739D489");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Produ__282DF8C2");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeCode).HasName("PK__OrderTyp__6FE06255E9EF9EEB");

            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.PickNo).HasName("PK__PickList__C80F569CA82E007A");

            entity.ToTable("PickList");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.PickDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__PickList__Reserv__3B40CD36");

            entity.HasOne(d => d.Status).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PickList__Status__3D2915A8");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__PickList__Wareho__3C34F16F");
        });

        modelBuilder.Entity<PickListDetail>(entity =>
        {
            entity.HasKey(e => new { e.PickNo, e.ProductCode }).HasName("PK__PickList__3AFBB6B8C775B6A5");

            entity.ToTable("PickListDetail");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PickListD__Locat__42E1EEFE");

            entity.HasOne(d => d.PickNoNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.PickNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__PickN__40F9A68C");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__Produ__41EDCAC5");
        });

        modelBuilder.Entity<ProductMaster>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__ProductM__2F4E024E53972E90");

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

            entity.HasOne(d => d.Category).WithMany(p => p.ProductMasters)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__ProductMa__Categ__59063A47");
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.SupplierCode, e.ProductCode }).HasName("PK__ProductS__B64A783E26A11EBB");

            entity.ToTable("ProductSupplier");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Produ__5EBF139D");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.SupplierCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Suppl__5DCAEF64");
        });

        modelBuilder.Entity<PutAway>(entity =>
        {
            entity.HasKey(e => e.PutAwayCode).HasName("PK__PutAway__4283C16535A255CE");

            entity.ToTable("PutAway");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PutAwayDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAway__Locatio__46B27FE2");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PutAway__OrderTy__45BE5BA9");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PutAway__Respons__47A6A41B");

            entity.HasOne(d => d.Status).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PutAway__StatusI__73852659");
        });

        modelBuilder.Entity<PutAwayDetail>(entity =>
        {
            entity.HasKey(e => new { e.PutAwayCode, e.ProductCode }).HasName("PK__PutAwayD__B07721419F04CF23");

            entity.ToTable("PutAwayDetail");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__Produ__4B7734FF");

            entity.HasOne(d => d.PutAwayCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.PutAwayCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__PutAw__4A8310C6");
        });

        modelBuilder.Entity<PutAwayRule>(entity =>
        {
            entity.HasKey(e => e.PutAwaysRuleCode).HasName("PK__PutAwayR__F3CC5F4DF4C4784B");

            entity.Property(e => e.PutAwaysRuleCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseToApply).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAwayRu__Locat__1AD3FDA4");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__PutAwayRu__Produ__19DFD96B");

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__PutAwayRu__Stora__1BC821DD");

            entity.HasOne(d => d.WarehouseToApplyNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.WarehouseToApply)
                .HasConstraintName("FK__PutAwayRu__Wareh__18EBB532");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationCode).HasName("PK__Reservat__2081C0BAD6527285");

            entity.ToTable("Reservation");

            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.OrderId)
                .HasMaxLength(100)
                .HasColumnName("OrderID");
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Reservati__Order__32AB8735");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Reservati__Statu__31B762FC");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Reservati__Wareh__339FAB6E");
        });

        modelBuilder.Entity<ReservationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3213E83F0AF03D60");

            entity.ToTable("ReservationDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.ReservationCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__Reservati__Locat__3864608B");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Reservati__Produ__37703C52");

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__Reservati__Reser__367C1819");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE204305E889D2");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.HasKey(e => e.StockInCode).HasName("PK__StockIn__3FAEE5C5E61C1C30");

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
                .HasConstraintName("FK__StockIn__OrderTy__76969D2E");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockIn__Respons__797309D9");

            entity.HasOne(d => d.Status).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockIn__StatusI__7A672E12");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.SupplierCode)
                .HasConstraintName("FK__StockIn__Supplie__787EE5A0");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockIn__Warehou__778AC167");
        });

        modelBuilder.Entity<StockInDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockInCode, e.ProductCode }).HasName("PK__StockInD__CD5A05E178D5F0DD");

            entity.ToTable("StockInDetail");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Produ__7F2BE32F");

            entity.HasOne(d => d.StockInCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.StockInCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Stock__7E37BEF6");
        });

        modelBuilder.Entity<StockOut>(entity =>
        {
            entity.HasKey(e => e.StockOutCode).HasName("PK__StockOut__C9B8C856F9033338");

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
                .HasConstraintName("FK__StockOut__Custom__03F0984C");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockOut__OrderT__02084FDA");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockOut__Respon__04E4BC85");

            entity.HasOne(d => d.Status).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockOut__Status__05D8E0BE");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockOut__Wareho__02FC7413");
        });

        modelBuilder.Entity<StockOutDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockOutCode, e.ProductCode }).HasName("PK__StockOut__3B4C2872242DC13F");

            entity.ToTable("StockOutDetail");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Produ__0A9D95DB");

            entity.HasOne(d => d.StockOutCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.StockOutCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Stock__09A971A2");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.StocktakeCode).HasName("PK__Stocktak__66FC5708CCC1B8D0");

            entity.ToTable("Stocktake");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StocktakeDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Stocktake__Respo__5AB9788F");

            entity.HasOne(d => d.Status).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Stocktake__Statu__59C55456");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Stocktake__Wareh__5BAD9CC8");
        });

        modelBuilder.Entity<StocktakeDetail>(entity =>
        {
            entity.HasKey(e => e.StocktakeCode).HasName("PK__Stocktak__66FC570838951E96");

            entity.ToTable("StocktakeDetail");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__Stocktake__Locat__5F7E2DAC");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Stocktake__Count__5E8A0973");
        });

        modelBuilder.Entity<StorageProduct>(entity =>
        {
            entity.HasKey(e => e.StorageProductId).HasName("PK__StorageP__6066276FC81CA06D");

            entity.ToTable("StorageProduct");

            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StorageProducts)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__StoragePr__Produ__6754599E");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SupplierCode).HasName("PK__Supplier__44BE981ABBDCF4D1");

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.SupplierPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.TransferCode).HasName("PK__Transfer__CE99A4C48A9F4DCE");

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
                .HasConstraintName("FK__Transfer__FromWa__0D7A0286");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Transfer__OrderT__114A936A");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Transfer__Respon__0F624AF8");

            entity.HasOne(d => d.Status).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Transfer__Status__10566F31");

            entity.HasOne(d => d.ToWarehouseCodeNavigation).WithMany(p => p.TransferToWarehouseCodeNavigations)
                .HasForeignKey(d => d.ToWarehouseCode)
                .HasConstraintName("FK__Transfer__ToWare__0E6E26BF");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransferCode, e.ProductCode }).HasName("PK__Transfer__3C6D44E0325A8D80");

            entity.ToTable("TransferDetail");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Produ__160F4887");

            entity.HasOne(d => d.TransferCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Trans__151B244E");
        });

        modelBuilder.Entity<WarehouseMaster>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("PK__Warehous__1686A0579D5BFEA4");

            entity.ToTable("WarehouseMaster");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
        });

        modelBuilder.Entity<WorkCenterMaster>(entity =>
        {
            entity.HasKey(e => new { e.WorkCenterCode, e.WarehouseCode }).HasName("PK__WorkCent__177E3C988B0D9FAE");

            entity.ToTable("WorkCenterMaster");

            entity.Property(e => e.WorkCenterCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.WorkCenterName).HasMaxLength(100);

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.WorkCenterMasters)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkCente__Wareh__4E53A1AA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
