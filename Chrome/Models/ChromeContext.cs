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

    public virtual DbSet<Bommaster> Bommasters { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }

    public virtual DbSet<CustomerProduct> CustomerProducts { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<GroupFunction> GroupFunctions { get; set; }

    public virtual DbSet<GroupManagement> GroupManagements { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<LocationMaster> LocationMasters { get; set; }

    public virtual DbSet<ManufacturingOrder> ManufacturingOrders { get; set; }

    public virtual DbSet<ManufacturingOrderDetail> ManufacturingOrderDetails { get; set; }

    public virtual DbSet<Movement> Movements { get; set; }

    public virtual DbSet<MovementDetail> MovementDetails { get; set; }

    public virtual DbSet<OrderType> OrderTypes { get; set; }

    public virtual DbSet<PickList> PickLists { get; set; }

    public virtual DbSet<PickListDetail> PickListDetails { get; set; }

    public virtual DbSet<ProductMaster> ProductMasters { get; set; }

    public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<PutAway> PutAways { get; set; }

    public virtual DbSet<PutAwayDetail> PutAwayDetails { get; set; }

    public virtual DbSet<PutAwayRule> PutAwayRules { get; set; }

    public virtual DbSet<Replenish> Replenishes { get; set; }

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
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Chrome;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountManagement>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F284572F3F705A");

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
            entity.HasKey(e => new { e.Bomcode, e.ComponentCode, e.BomVersion }).HasName("PK__BOM_Comp__568017D4C1B41DBB");

            entity.ToTable("BOM_Component");

            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.ComponentCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.BomComponents)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Compo__Compo__395884C4");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.BomComponents)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Component__3864608B");
        });

        modelBuilder.Entity<Bommaster>(entity =>
        {
            entity.HasKey(e => new { e.Bomcode, e.Bomversion }).HasName("PK__BOMMaste__1DABFE87D692A92C");

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
                .HasConstraintName("FK__BOMMaster__Produ__3587F3E0");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BE8D84072");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__066785204BBD45A1");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<CustomerProduct>(entity =>
        {
            entity.HasKey(e => new { e.CustomerCode, e.ProductCode }).HasName("PK__Customer__F4936504FA135990");

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
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF91830BD09F4");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId, e.ApplicableLocation }).HasName("PK__GroupFun__CAF59E7E16EC6FB1");

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
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30A250AE2B3");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.ProductCode, e.LocationCode, e.Lotno }).HasName("PK__Inventor__8DD47BF844678FC0");

            entity.ToTable("Inventory");

            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ReceiveDate).HasColumnType("datetime");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__6E01572D");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__6EF57B66");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationCode).HasName("PK__Location__DDB144D408E5D2BE");

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

        modelBuilder.Entity<ManufacturingOrder>(entity =>
        {
            entity.HasKey(e => e.ManufacturingOrderCode).HasName("PK__Manufact__5DC020C370713DF1");

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
                .HasConstraintName("FK__Manufactu__Order__57DD0BE4");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Manufactu__Wareh__56E8E7AB");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Manufactu__Respo__5AB9788F");

            entity.HasOne(d => d.Status).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Manufactu__Statu__59C55456");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Manufactu__Wareh__5BAD9CC8");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.ManufacturingOrders)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .HasConstraintName("FK__ManufacturingOrd__58D1301D");
        });

        modelBuilder.Entity<ManufacturingOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.ManufacturingOrderCode, e.ComponentCode }).HasName("PK__Manufact__D558EF7CAC283A88");

            entity.ToTable("ManufacturingOrderDetail");

            entity.Property(e => e.ManufacturingOrderCode).HasMaxLength(100);
            entity.Property(e => e.ComponentCode).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.ManufacturingOrderDetails)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufactu__Compo__5E8A0973");

            entity.HasOne(d => d.ManufacturingOrderCodeNavigation).WithMany(p => p.ManufacturingOrderDetails)
                .HasForeignKey(d => d.ManufacturingOrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manufactu__Manuf__5F7E2DAC");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.MovementCode).HasName("PK__Movement__8B71BE1C0FA79F06");

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
                .HasConstraintName("FK__Movement__FromLo__2BFE89A6");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Movement__OrderT__2A164134");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Movement__Respon__2DE6D218");

            entity.HasOne(d => d.Status).WithMany(p => p.Movements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Movement__Status__2EDAF651");

            entity.HasOne(d => d.ToLocationNavigation).WithMany(p => p.MovementToLocationNavigations)
                .HasForeignKey(d => d.ToLocation)
                .HasConstraintName("FK__Movement__ToLoca__2CF2ADDF");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Movement__Wareho__2B0A656D");
        });

        modelBuilder.Entity<MovementDetail>(entity =>
        {
            entity.HasKey(e => new { e.MovementCode, e.ProductCode }).HasName("PK__Movement__79855E38298C9CC6");

            entity.ToTable("MovementDetail");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.MovementCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.MovementCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Movem__31B762FC");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Produ__32AB8735");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeCode).HasName("PK__OrderTyp__6FE0625571C7ED27");

            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.PickNo).HasName("PK__PickList__C80F569CB466A5E7");

            entity.ToTable("PickList");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.PickDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__PickList__Reserv__43D61337");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PickList__Respon__45BE5BA9");

            entity.HasOne(d => d.Status).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PickList__Status__44CA3770");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__PickList__Wareho__46B27FE2");
        });

        modelBuilder.Entity<PickListDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PickList__3214EC2731D88CCB");

            entity.ToTable("PickListDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.PickNoNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.PickNo)
                .HasConstraintName("FK__PickListD__PickN__4A8310C6");
        });

        modelBuilder.Entity<ProductMaster>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__ProductM__2F4E024EC96ED19C");

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
            entity.HasKey(e => new { e.SupplierCode, e.ProductCode }).HasName("PK__ProductS__B64A783EFD844CE6");

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

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderCode).HasName("PK__Purchase__E282C5B5B8C5BAF0");

            entity.ToTable("PurchaseOrder");

            entity.Property(e => e.PurchaseOrderCode).HasMaxLength(100);
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.Status).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PurchaseO__Statu__778AC167");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierCode)
                .HasConstraintName("FK__PurchaseO__Suppl__76969D2E");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__PurchaseO__Wareh__75A278F5");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.PurchaseOrderCode, e.ProductCode }).HasName("PK__Purchase__107625913BA0A098");

            entity.ToTable("PurchaseOrderDetail");

            entity.Property(e => e.PurchaseOrderCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);
            entity.Property(e => e.QuantityReceived).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Produ__7D439ABD");

            entity.HasOne(d => d.PurchaseOrderCodeNavigation).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.PurchaseOrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Purch__7C4F7684");
        });

        modelBuilder.Entity<PutAway>(entity =>
        {
            entity.HasKey(e => e.PutAwayCode).HasName("PK__PutAway__4283C16550D3FDF5");

            entity.ToTable("PutAway");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PutAwayDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAway__Locatio__4E53A1AA");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PutAway__OrderTy__4D5F7D71");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PutAway__Respons__4F47C5E3");

            entity.HasOne(d => d.Status).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PutAway__StatusI__503BEA1C");
        });

        modelBuilder.Entity<PutAwayDetail>(entity =>
        {
            entity.HasKey(e => new { e.PutAwayCode, e.ProductCode, e.LotNo }).HasName("PK__PutAwayD__77364195B03860F9");

            entity.ToTable("PutAwayDetail");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__Produ__531856C7");

            entity.HasOne(d => d.PutAwayCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.PutAwayCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__PutAw__540C7B00");
        });

        modelBuilder.Entity<PutAwayRule>(entity =>
        {
            entity.HasKey(e => e.PutAwaysRuleCode).HasName("PK__PutAwayR__F3CC5F4D38354979");

            entity.Property(e => e.PutAwaysRuleCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseToApply).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAwayRu__Locat__2645B050");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__PutAwayRu__Produ__25518C17");

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__PutAwayRu__Stora__2739D489");

            entity.HasOne(d => d.WarehouseToApplyNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.WarehouseToApply)
                .HasConstraintName("FK__PutAwayRu__Wareh__245D67DE");
        });

        modelBuilder.Entity<Replenish>(entity =>
        {
            entity.HasKey(e => new { e.ProductCode, e.WarehouseCode }).HasName("PK__Replenis__4E26684B93AECFD0");

            entity.ToTable("Replenish");

            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.MaxQuantity).HasColumnName("maxQuantity");
            entity.Property(e => e.MinQuantity).HasColumnName("minQuantity");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Replenishes)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Replenish__Produ__6BE40491");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Replenishes)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Replenish__Wareh__6CD828CA");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationCode).HasName("PK__Reservat__2081C0BABC6B5E30");

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
                .HasConstraintName("FK__Reservati__Order__3E1D39E1");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Reservati__Statu__3D2915A8");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Reservati__Wareh__3C34F16F");
        });

        modelBuilder.Entity<ReservationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC27493DBA55");

            entity.ToTable("ReservationDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.ReservationCode).HasMaxLength(100);

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__Reservati__Reser__40F9A68C");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE2043A48C92B5");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.HasKey(e => e.StockInCode).HasName("PK__StockIn__3FAEE5C5762B5905");

            entity.ToTable("StockIn");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.OrderDeadline).HasColumnType("datetime");
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PurchaseOrderCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockIn__OrderTy__01142BA1");

            entity.HasOne(d => d.PurchaseOrderCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.PurchaseOrderCode)
                .HasConstraintName("FK__StockIn__Purchas__00200768");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockIn__Respons__02FC7413");

            entity.HasOne(d => d.Status).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockIn__StatusI__03F0984C");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockIn__Warehou__02084FDA");
        });

        modelBuilder.Entity<StockInDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockInCode, e.ProductCode, e.LotNo }).HasName("PK__StockInD__0A1B65357003C96F");

            entity.ToTable("StockInDetail");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Produ__08B54D69");

            entity.HasOne(d => d.StockInCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.StockInCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Stock__07C12930");
        });

        modelBuilder.Entity<StockOut>(entity =>
        {
            entity.HasKey(e => e.StockOutCode).HasName("PK__StockOut__C9B8C8568B53EA22");

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
                .HasConstraintName("FK__StockOut__Custom__0D7A0286");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockOut__OrderT__0B91BA14");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockOut__Respon__0E6E26BF");

            entity.HasOne(d => d.Status).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockOut__Status__0F624AF8");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockOut__Wareho__0C85DE4D");
        });

        modelBuilder.Entity<StockOutDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockOutCode, e.ProductCode }).HasName("PK__StockOut__3B4C28729205FA33");

            entity.ToTable("StockOutDetail");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Produ__14270015");

            entity.HasOne(d => d.StockOutCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.StockOutCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Stock__1332DBDC");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.StocktakeCode).HasName("PK__Stocktak__66FC570861D7DEF8");

            entity.ToTable("Stocktake");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StocktakeDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Stocktake__Respo__634EBE90");

            entity.HasOne(d => d.Status).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Stocktake__Statu__625A9A57");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Stocktake__Wareh__6442E2C9");
        });

        modelBuilder.Entity<StocktakeDetail>(entity =>
        {
            entity.HasKey(e => new { e.StocktakeCode, e.ProductCode, e.Lotno, e.LocationCode }).HasName("PK__Stocktak__261401C3482E82A3");

            entity.ToTable("StocktakeDetail");

            entity.Property(e => e.StocktakeCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Locat__690797E6");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Produ__681373AD");

            entity.HasOne(d => d.StocktakeCodeNavigation).WithMany(p => p.StocktakeDetails)
                .HasForeignKey(d => d.StocktakeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stocktake__Stock__671F4F74");
        });

        modelBuilder.Entity<StorageProduct>(entity =>
        {
            entity.HasKey(e => e.StorageProductId).HasName("PK__StorageP__6066276FDAD8B359");

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
            entity.HasKey(e => e.SupplierCode).HasName("PK__Supplier__44BE981AF0572BC0");

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.SupplierPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.TransferCode).HasName("PK__Transfer__CE99A4C4525C884D");

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
                .HasConstraintName("FK__Transfer__FromRe__19DFD96B");

            entity.HasOne(d => d.FromWarehouseCodeNavigation).WithMany(p => p.TransferFromWarehouseCodeNavigations)
                .HasForeignKey(d => d.FromWarehouseCode)
                .HasConstraintName("FK__Transfer__FromWa__17036CC0");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Transfer__OrderT__1BC821DD");

            entity.HasOne(d => d.Status).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Transfer__Status__1AD3FDA4");

            entity.HasOne(d => d.ToResponsibleNavigation).WithMany(p => p.TransferToResponsibleNavigations)
                .HasForeignKey(d => d.ToResponsible)
                .HasConstraintName("FK__Transfer__ToResp__18EBB532");

            entity.HasOne(d => d.ToWarehouseCodeNavigation).WithMany(p => p.TransferToWarehouseCodeNavigations)
                .HasForeignKey(d => d.ToWarehouseCode)
                .HasConstraintName("FK__Transfer__ToWare__17F790F9");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransferCode, e.ProductCode }).HasName("PK__Transfer__3C6D44E00D82198D");

            entity.ToTable("TransferDetail");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.QuantityInBounded).HasDefaultValue(0.0);
            entity.Property(e => e.QuantityOutBounded).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Produ__2180FB33");

            entity.HasOne(d => d.TransferCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Trans__208CD6FA");
        });

        modelBuilder.Entity<WarehouseMaster>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("PK__Warehous__1686A057EFF67663");

            entity.ToTable("WarehouseMaster");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
