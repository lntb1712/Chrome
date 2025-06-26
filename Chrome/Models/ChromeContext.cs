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

    public virtual DbSet<ManufacturingOrder> ManufacturingOrders { get; set; }

    public virtual DbSet<ManufacturingOrderDetail> ManufacturingOrderDetails { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Chrome;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountManagement>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F28457CB15F2DF");

            entity.ToTable("AccountManagement");

            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");

            entity.HasOne(d => d.Group).WithMany(p => p.AccountManagements)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__AccountMa__Group__151B244E");
        });

        modelBuilder.Entity<BomComponent>(entity =>
        {
            entity.HasKey(e => new { e.Bomcode, e.ComponentCode, e.BomVersion }).HasName("PK__BOM_Comp__568017D45A2463B0");

            entity.ToTable("BOM_Component");

            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.ComponentCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.BomComponents)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Compo__Compo__160F4887");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.BomComponents)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Component__17036CC0");
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
            entity.HasKey(e => new { e.Bomcode, e.Bomversion }).HasName("PK__BOMMaste__1DABFE874AF9D326");

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
                .HasConstraintName("FK__BOMMaster__Produ__17F790F9");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BE6972720");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__06678520ECAE1374");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<CustomerProduct>(entity =>
        {
            entity.HasKey(e => new { e.CustomerCode, e.ProductCode }).HasName("PK__Customer__F4936504457F0697");

            entity.ToTable("CustomerProduct");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.CustomerCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.CustomerCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Custo__18EBB532");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Produ__19DFD96B");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF9185A103009");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId, e.ApplicableLocation }).HasName("PK__GroupFun__CAF59E7E933D7B7A");

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
                .HasConstraintName("FK__GroupFunc__Appli__1AD3FDA4");

            entity.HasOne(d => d.Function).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Funct__1BC821DD");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Group__1CBC4616");
        });

        modelBuilder.Entity<GroupManagement>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30AB6E1B312");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseCode, e.LocationCode, e.ProductCode, e.Lotno }).HasName("PK__Inventor__BB86ECC7470AC1A3");

            entity.ToTable("Inventory");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ReceiveDate).HasColumnType("datetime");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__1DB06A4F");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__1EA48E88");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__1F98B2C1");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationCode).HasName("PK__Location__DDB144D433F0CAD8");

            entity.ToTable("LocationMaster");

            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.IsEmpty).HasColumnName("isEmpty");
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__LocationM__Stora__208CD6FA");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__LocationM__Wareh__2180FB33");
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

        modelBuilder.Entity<ManufacturingOrder>(entity =>
        {
            entity.HasKey(e => e.ManufacturingOrderCode).HasName("PK__Manufact__5DC020C38956F6D4");

            entity.ToTable("ManufacturingOrder");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);
            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.ScheduleDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Manufactu__Order__22751F6C");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Manufactu__Produ__236943A5");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Manufactu__Respo__245D67DE");

            entity.HasOne(d => d.Status).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Manufactu__Statu__25518C17");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Manufactu__Wareh__2645B050");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .HasConstraintName("FK__ManufacturingOrd__2739D489");
        });

        modelBuilder.Entity<ManufacturingOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.ManufacturingOrderCode, e.ComponentCode }).HasName("PK__Manufact__D558EF7C5FBF4FB9");

            entity.ToTable("ManufacturingOrderDetail");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.ComponentCode).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.ManufacturingOrderDetails)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufactu__Compo__282DF8C2");

            entity.HasOne(d => d.ManufacturingOrderCodeNavigation).WithMany(p => p.ManufacturingOrderDetails)
                .HasForeignKey(d => d.ManufacturingOrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufactu__Manuf__29221CFB");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.MovementCode).HasName("PK__Movement__8B71BE1C2D8950F4");

            entity.ToTable("Movement");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.FromLocation).HasMaxLength(100);
            entity.Property(e => e.MovementDate).HasColumnType("datetime");
            entity.Property(e => e.MovementDescription).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToLocation).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.FromLocationNavigation).WithMany(p => p.MovementFromLocationNavigations)
                .HasForeignKey(d => d.FromLocation)
                .HasConstraintName("FK__Movement__FromLo__2A164134");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Movement__OrderT__2B0A656D");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Movement__Respon__2BFE89A6");

            entity.HasOne(d => d.Status).WithMany(p => p.Movements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Movement__Status__2CF2ADDF");

            entity.HasOne(d => d.ToLocationNavigation).WithMany(p => p.MovementToLocationNavigations)
                .HasForeignKey(d => d.ToLocation)
                .HasConstraintName("FK__Movement__ToLoca__2DE6D218");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Movement__Wareho__2EDAF651");
        });

        modelBuilder.Entity<MovementDetail>(entity =>
        {
            entity.HasKey(e => new { e.MovementCode, e.ProductCode }).HasName("PK__Movement__79855E385A48F8C8");

            entity.ToTable("MovementDetail");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.MovementCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.MovementCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Movem__2FCF1A8A");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Produ__30C33EC3");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeCode).HasName("PK__OrderTyp__6FE06255B9D50731");

            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.PickNo).HasName("PK__PickList__C80F569C37500F27");

            entity.ToTable("PickList");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.PickDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__PickList__Reserv__31B762FC");

            entity.HasOne(d => d.Status).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PickList__Status__32AB8735");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__PickList__Wareho__339FAB6E");
        });

        modelBuilder.Entity<PickListDetail>(entity =>
        {
            entity.HasKey(e => new { e.PickNo, e.ProductCode, e.LotNo }).HasName("PK__PickList__FDBAD66CDC599D89");

            entity.ToTable("PickListDetail");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PickListD__Locat__3493CFA7");

            entity.HasOne(d => d.PickNoNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.PickNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__PickN__3587F3E0");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__Produ__367C1819");
        });

        modelBuilder.Entity<ProductMaster>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__ProductM__2F4E024EBE729325");

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
                .HasConstraintName("FK__ProductMa__Categ__37703C52");
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.SupplierCode, e.ProductCode }).HasName("PK__ProductS__B64A783ECBDB4BE3");

            entity.ToTable("ProductSupplier");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Produ__3864608B");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.SupplierCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Suppl__395884C4");
        });

        modelBuilder.Entity<PutAway>(entity =>
        {
            entity.HasKey(e => e.PutAwayCode).HasName("PK__PutAway__4283C165A02D1B3B");

            entity.ToTable("PutAway");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PutAwayDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAway__Locatio__3A4CA8FD");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PutAway__OrderTy__3B40CD36");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PutAway__Respons__3C34F16F");

            entity.HasOne(d => d.Status).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PutAway__StatusI__3D2915A8");
        });

        modelBuilder.Entity<PutAwayDetail>(entity =>
        {
            entity.HasKey(e => new { e.PutAwayCode, e.ProductCode }).HasName("PK__PutAwayD__B077214134F2214F");

            entity.ToTable("PutAwayDetail");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__Produ__3E1D39E1");

            entity.HasOne(d => d.PutAwayCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.PutAwayCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__PutAw__3F115E1A");
        });

        modelBuilder.Entity<PutAwayRule>(entity =>
        {
            entity.HasKey(e => e.PutAwaysRuleCode).HasName("PK__PutAwayR__F3CC5F4D7E33657F");

            entity.Property(e => e.PutAwaysRuleCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseToApply).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAwayRu__Locat__40058253");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__PutAwayRu__Produ__40F9A68C");

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__PutAwayRu__Stora__41EDCAC5");

            entity.HasOne(d => d.WarehouseToApplyNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.WarehouseToApply)
                .HasConstraintName("FK__PutAwayRu__Wareh__42E1EEFE");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationCode).HasName("PK__Reservat__2081C0BA9FCB79F1");

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
                .HasConstraintName("FK__Reservati__Order__43D61337");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Reservati__Statu__44CA3770");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Reservati__Wareh__45BE5BA9");
        });

        modelBuilder.Entity<ReservationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3213E83FAEDECE58");

            entity.ToTable("ReservationDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.ReservationCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__Reservati__Locat__46B27FE2");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Reservati__Produ__47A6A41B");

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__Reservati__Reser__489AC854");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE20430FCF0482");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.HasKey(e => e.StockInCode).HasName("PK__StockIn__3FAEE5C51F381F09");

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
                .HasConstraintName("FK__StockIn__OrderTy__498EEC8D");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockIn__Respons__4A8310C6");

            entity.HasOne(d => d.Status).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockIn__StatusI__4B7734FF");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.SupplierCode)
                .HasConstraintName("FK__StockIn__Supplie__4C6B5938");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockIn__Warehou__4D5F7D71");
        });

        modelBuilder.Entity<StockInDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockInCode, e.ProductCode }).HasName("PK__StockInD__CD5A05E17BF43782");

            entity.ToTable("StockInDetail");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Produ__4E53A1AA");

            entity.HasOne(d => d.StockInCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.StockInCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Stock__4F47C5E3");
        });

        modelBuilder.Entity<StockOut>(entity =>
        {
            entity.HasKey(e => e.StockOutCode).HasName("PK__StockOut__C9B8C85606FD7037");

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
                .HasConstraintName("FK__StockOut__Custom__503BEA1C");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockOut__OrderT__51300E55");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockOut__Respon__5224328E");

            entity.HasOne(d => d.Status).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockOut__Status__531856C7");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockOut__Wareho__540C7B00");
        });

        modelBuilder.Entity<StockOutDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockOutCode, e.ProductCode }).HasName("PK__StockOut__3B4C28728ECC0AAA");

            entity.ToTable("StockOutDetail");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Produ__55009F39");

            entity.HasOne(d => d.StockOutCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.StockOutCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Stock__55F4C372");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.StocktakeCode).HasName("PK__Stocktak__66FC570896E450AC");

            entity.ToTable("Stocktake");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StocktakeDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Stocktake__Respo__56E8E7AB");

            entity.HasOne(d => d.Status).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Stocktake__Statu__57DD0BE4");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Stocktake__Wareh__58D1301D");
        });

        modelBuilder.Entity<StocktakeDetail>(entity =>
        {
            entity.HasKey(e => new { e.StocktakeCode, e.ProductCode, e.Lotno, e.LocationCode }).HasName("PK__Stocktak__261401C3FD1F6307");

            entity.ToTable("StocktakeDetail");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Locat__59C55456");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Produ__5AB9788F");

            entity.HasOne(d => d.StocktakeCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.StocktakeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Stock__5BAD9CC8");
        });

        modelBuilder.Entity<StorageProduct>(entity =>
        {
            entity.HasKey(e => e.StorageProductId).HasName("PK__StorageP__6066276FE6F36854");

            entity.ToTable("StorageProduct");

            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StorageProducts)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__StoragePr__Produ__5CA1C101");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SupplierCode).HasName("PK__Supplier__44BE981A0A16026A");

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.SupplierPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.TransferCode).HasName("PK__Transfer__CE99A4C47A159288");

            entity.ToTable("Transfer");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.FromResponsible).HasMaxLength(100);
            entity.Property(e => e.FromWarehouseCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToResponsible).HasMaxLength(100);
            entity.Property(e => e.ToWarehouseCode).HasMaxLength(100);
            entity.Property(e => e.TransferDate).HasColumnType("datetime");

            entity.HasOne(d => d.FromResponsibleNavigation).WithMany(p => p.TransferFromResponsibleNavigations)
                .HasForeignKey(d => d.FromResponsible)
                .HasConstraintName("FK__Transfer__FromRe__5D95E53A");

            entity.HasOne(d => d.FromWarehouseCodeNavigation).WithMany(p => p.TransferFromWarehouseCodeNavigations)
                .HasForeignKey(d => d.FromWarehouseCode)
                .HasConstraintName("FK__Transfer__FromWa__5E8A0973");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Transfer__OrderT__5F7E2DAC");

            entity.HasOne(d => d.Status).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Transfer__Status__607251E5");

            entity.HasOne(d => d.ToResponsibleNavigation).WithMany(p => p.TransferToResponsibleNavigations)
                .HasForeignKey(d => d.ToResponsible)
                .HasConstraintName("FK__Transfer__ToResp__6166761E");

            entity.HasOne(d => d.ToWarehouseCodeNavigation).WithMany(p => p.TransferToWarehouseCodeNavigations)
                .HasForeignKey(d => d.ToWarehouseCode)
                .HasConstraintName("FK__Transfer__ToWare__625A9A57");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransferCode, e.ProductCode }).HasName("PK__Transfer__3C6D44E0047E59CF");

            entity.ToTable("TransferDetail");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.QuantityInBounded).HasDefaultValue(0.0);
            entity.Property(e => e.QuantityOutBounded).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Produ__634EBE90");

            entity.HasOne(d => d.TransferCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Trans__6442E2C9");
        });

        modelBuilder.Entity<WarehouseMaster>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("PK__Warehous__1686A057FAE4C1A4");

            entity.ToTable("WarehouseMaster");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
