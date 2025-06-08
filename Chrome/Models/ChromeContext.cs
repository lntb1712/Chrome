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
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F2845785FDF843");

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
            entity.HasKey(e => new { e.Bomcode, e.ComponentCode, e.BomVersion }).HasName("PK__BOM_Comp__568017D4F8AE6327");

            entity.ToTable("BOM_Component");

            entity.Property(e => e.Bomcode)
                .HasMaxLength(100)
                .HasColumnName("BOMCode");
            entity.Property(e => e.ComponentCode).HasMaxLength(100);
            entity.Property(e => e.BomVersion).HasMaxLength(100);

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.BomComponents)
                .HasForeignKey(d => d.ComponentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Compo__Compo__2FCF1A8A");

            entity.HasOne(d => d.Bommaster).WithMany(p => p.BomComponents)
                .HasForeignKey(d => new { d.Bomcode, d.BomVersion })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM_Component__2EDAF651");
        });

        modelBuilder.Entity<Bommaster>(entity =>
        {
            entity.HasKey(e => new { e.Bomcode, e.Bomversion }).HasName("PK__BOMMaste__1DABFE87928C596D");

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
                .HasConstraintName("FK__BOMMaster__Produ__2BFE89A6");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BB69CE2F2");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(100)
                .HasColumnName("CategoryID");
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__06678520C93CA4D1");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<CustomerProduct>(entity =>
        {
            entity.HasKey(e => new { e.CustomerCode, e.ProductCode }).HasName("PK__Customer__F493650458F52CF4");

            entity.ToTable("CustomerProduct");

            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.CustomerCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.CustomerCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Custo__6477ECF3");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.CustomerProducts)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Produ__656C112C");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF918E0DD6420");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId, e.ApplicableLocation }).HasName("PK__GroupFun__CAF59E7E7DEC2490");

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
                .HasConstraintName("FK__GroupFunc__Appli__5535A963");

            entity.HasOne(d => d.Function).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Funct__5441852A");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Group__534D60F1");
        });

        modelBuilder.Entity<GroupManagement>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30AB8C8639C");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseCode, e.LocationCode, e.ProductCode, e.Lotno }).HasName("PK__Inventor__BB86ECC73C301A7C");

            entity.ToTable("Inventory");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__6FE99F9F");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__70DDC3D8");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__6EF57B66");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationCode).HasName("PK__Location__DDB144D48FF4FA26");

            entity.ToTable("LocationMaster");

            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.IsEmpty).HasColumnName("isEmpty");
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__LocationM__Stora__6C190EBB");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.LocationMasters)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__LocationM__Wareh__6B24EA82");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.MovementCode).HasName("PK__Movement__8B71BE1C830939CE");

            entity.ToTable("Movement");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.FromLocation).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.ToLocation).HasMaxLength(100);
            entity.Property(e => e.TransferDate).HasColumnType("datetime");
            entity.Property(e => e.TransferDescription).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.FromLocationNavigation).WithMany(p => p.MovementFromLocationNavigations)
                .HasForeignKey(d => d.FromLocation)
                .HasConstraintName("FK__Movement__FromLo__2180FB33");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Movement__OrderT__1F98B2C1");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Movement__Respon__236943A5");

            entity.HasOne(d => d.Status).WithMany(p => p.Movements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Movement__Status__245D67DE");

            entity.HasOne(d => d.ToLocationNavigation).WithMany(p => p.MovementToLocationNavigations)
                .HasForeignKey(d => d.ToLocation)
                .HasConstraintName("FK__Movement__ToLoca__22751F6C");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Movement__Wareho__208CD6FA");
        });

        modelBuilder.Entity<MovementDetail>(entity =>
        {
            entity.HasKey(e => new { e.MovementCode, e.ProductCode }).HasName("PK__Movement__79855E389D558DB4");

            entity.ToTable("MovementDetail");

            entity.Property(e => e.MovementCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.MovementCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.MovementCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Movem__282DF8C2");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.MovementDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovementD__Produ__29221CFB");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeCode).HasName("PK__OrderTyp__6FE0625560C56849");

            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.PickNo).HasName("PK__PickList__C80F569CC4DB478F");

            entity.ToTable("PickList");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.PickDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__PickList__Reserv__3D2915A8");

            entity.HasOne(d => d.Status).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PickList__Status__3F115E1A");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.PickLists)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__PickList__Wareho__3E1D39E1");
        });

        modelBuilder.Entity<PickListDetail>(entity =>
        {
            entity.HasKey(e => new { e.PickNo, e.ProductCode }).HasName("PK__PickList__3AFBB6B828BD9746");

            entity.ToTable("PickListDetail");

            entity.Property(e => e.PickNo).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PickListD__Locat__44CA3770");

            entity.HasOne(d => d.PickNoNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.PickNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__PickN__42E1EEFE");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PickListDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PickListD__Produ__43D61337");
        });

        modelBuilder.Entity<ProductMaster>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__ProductM__2F4E024E35220D54");

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
                .HasConstraintName("FK__ProductMa__Categ__59FA5E80");
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.SupplierCode, e.ProductCode }).HasName("PK__ProductS__B64A783EF8077311");

            entity.ToTable("ProductSupplier");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Produ__5FB337D6");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.SupplierCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Suppl__5EBF139D");
        });

        modelBuilder.Entity<PutAway>(entity =>
        {
            entity.HasKey(e => e.PutAwayCode).HasName("PK__PutAway__4283C165AFC5044F");

            entity.ToTable("PutAway");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.PutAwayDate).HasColumnType("datetime");
            entity.Property(e => e.Responsible).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAway__Locatio__489AC854");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__PutAway__OrderTy__47A6A41B");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.PutAways)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__PutAway__Respons__498EEC8D");
        });

        modelBuilder.Entity<PutAwayDetail>(entity =>
        {
            entity.HasKey(e => new { e.PutAwayCode, e.ProductCode }).HasName("PK__PutAwayD__B0772141604528F1");

            entity.ToTable("PutAwayDetail");

            entity.Property(e => e.PutAwayCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__Produ__4D5F7D71");

            entity.HasOne(d => d.PutAwayCodeNavigation).WithMany(p => p.PutAwayDetails)
                .HasForeignKey(d => d.PutAwayCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PutAwayDe__PutAw__4C6B5938");
        });

        modelBuilder.Entity<PutAwayRule>(entity =>
        {
            entity.HasKey(e => e.PutAwaysRuleCode).HasName("PK__PutAwayR__F3CC5F4D6388BF17");

            entity.Property(e => e.PutAwaysRuleCode).HasMaxLength(100);
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.WarehouseToApply).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__PutAwayRu__Locat__1BC821DD");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__PutAwayRu__Produ__1AD3FDA4");

            entity.HasOne(d => d.StorageProduct).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.StorageProductId)
                .HasConstraintName("FK__PutAwayRu__Stora__1CBC4616");

            entity.HasOne(d => d.WarehouseToApplyNavigation).WithMany(p => p.PutAwayRules)
                .HasForeignKey(d => d.WarehouseToApply)
                .HasConstraintName("FK__PutAwayRu__Wareh__19DFD96B");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationCode).HasName("PK__Reservat__2081C0BA32B24FD8");

            entity.ToTable("Reservation");

            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.OrderId)
                .HasMaxLength(100)
                .HasColumnName("OrderID");
            entity.Property(e => e.OrderTypeCode).HasMaxLength(100);
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Reservati__Order__339FAB6E");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Reservati__Statu__32AB8735");
        });

        modelBuilder.Entity<ReservationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3213E83F6BC247EB");

            entity.ToTable("ReservationDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LocationCode).HasMaxLength(100);
            entity.Property(e => e.Lotno).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantityprocessed).HasDefaultValue(0.0);
            entity.Property(e => e.ReservationCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseCode).HasMaxLength(100);

            entity.HasOne(d => d.LocationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.LocationCode)
                .HasConstraintName("FK__Reservati__Locat__3A4CA8FD");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__Reservati__Produ__3864608B");

            entity.HasOne(d => d.ReservationCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ReservationCode)
                .HasConstraintName("FK__Reservati__Reser__37703C52");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__Reservati__Wareh__395884C4");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE2043A9DC4804");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StockIn>(entity =>
        {
            entity.HasKey(e => e.StockInCode).HasName("PK__StockIn__3FAEE5C59FBFE5C0");

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
                .HasConstraintName("FK__StockIn__OrderTy__778AC167");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockIn__Respons__7A672E12");

            entity.HasOne(d => d.Status).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockIn__StatusI__7B5B524B");

            entity.HasOne(d => d.SupplierCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.SupplierCode)
                .HasConstraintName("FK__StockIn__Supplie__797309D9");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockIns)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockIn__Warehou__787EE5A0");
        });

        modelBuilder.Entity<StockInDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockInCode, e.ProductCode }).HasName("PK__StockInD__CD5A05E15B198266");

            entity.ToTable("StockInDetail");

            entity.Property(e => e.StockInCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.LotNo).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Produ__00200768");

            entity.HasOne(d => d.StockInCodeNavigation).WithMany(p => p.StockInDetails)
                .HasForeignKey(d => d.StockInCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockInDe__Stock__7F2BE32F");
        });

        modelBuilder.Entity<StockOut>(entity =>
        {
            entity.HasKey(e => e.StockOutCode).HasName("PK__StockOut__C9B8C8568602EF67");

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
                .HasConstraintName("FK__StockOut__Custom__04E4BC85");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__StockOut__OrderT__02FC7413");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__StockOut__Respon__05D8E0BE");

            entity.HasOne(d => d.Status).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StockOut__Status__06CD04F7");

            entity.HasOne(d => d.WarehouseCodeNavigation).WithMany(p => p.StockOuts)
                .HasForeignKey(d => d.WarehouseCode)
                .HasConstraintName("FK__StockOut__Wareho__03F0984C");
        });

        modelBuilder.Entity<StockOutDetail>(entity =>
        {
            entity.HasKey(e => new { e.StockOutCode, e.ProductCode }).HasName("PK__StockOut__3B4C287292F8FD14");

            entity.ToTable("StockOutDetail");

            entity.Property(e => e.StockOutCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Produ__0B91BA14");

            entity.HasOne(d => d.StockOutCodeNavigation).WithMany(p => p.StockOutDetails)
                .HasForeignKey(d => d.StockOutCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockOutD__Stock__0A9D95DB");
        });

        modelBuilder.Entity<StorageProduct>(entity =>
        {
            entity.HasKey(e => e.StorageProductId).HasName("PK__StorageP__6066276FC099483E");

            entity.ToTable("StorageProduct");

            entity.Property(e => e.StorageProductId)
                .HasMaxLength(100)
                .HasColumnName("StorageProductID");
            entity.Property(e => e.ProductCode).HasMaxLength(100);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StorageProducts)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK__StoragePr__Produ__68487DD7");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SupplierCode).HasName("PK__Supplier__44BE981AEE0DB344");

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierCode).HasMaxLength(100);
            entity.Property(e => e.SupplierPhone).HasMaxLength(12);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.TransferCode).HasName("PK__Transfer__CE99A4C4EE88A66B");

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
                .HasConstraintName("FK__Transfer__FromWa__0E6E26BF");

            entity.HasOne(d => d.OrderTypeCodeNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.OrderTypeCode)
                .HasConstraintName("FK__Transfer__OrderT__123EB7A3");

            entity.HasOne(d => d.ResponsibleNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.Responsible)
                .HasConstraintName("FK__Transfer__Respon__10566F31");

            entity.HasOne(d => d.Status).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Transfer__Status__114A936A");

            entity.HasOne(d => d.ToWarehouseCodeNavigation).WithMany(p => p.TransferToWarehouseCodeNavigations)
                .HasForeignKey(d => d.ToWarehouseCode)
                .HasConstraintName("FK__Transfer__ToWare__0F624AF8");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransferCode, e.ProductCode }).HasName("PK__Transfer__3C6D44E0E0513942");

            entity.ToTable("TransferDetail");

            entity.Property(e => e.TransferCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(0.0);

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Produ__17036CC0");

            entity.HasOne(d => d.TransferCodeNavigation).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferD__Trans__160F4887");
        });

        modelBuilder.Entity<WarehouseMaster>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("PK__Warehous__1686A057BF93A6A1");

            entity.ToTable("WarehouseMaster");

            entity.Property(e => e.WarehouseCode).HasMaxLength(100);
            entity.Property(e => e.WarehouseManager).HasMaxLength(100);

            entity.HasOne(d => d.WarehouseManagerNavigation).WithMany(p => p.WarehouseMasters)
                .HasForeignKey(d => d.WarehouseManager)
                .HasConstraintName("FK__Warehouse__Wareh__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
