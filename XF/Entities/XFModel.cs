namespace XF.Entities
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class XFModel : DbContext
    {
        public XFModel()
            : base("name=XFModel")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<InvoiceStatu> InvoiceStatus { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderStatu> PurchaseOrderStatus { get; set; }
        public virtual DbSet<Storage> Storages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Products)
                .WithMany(e => e.Branches)
                .Map(m => m.ToTable("ProductsBranch").MapLeftKey("BranchId").MapRightKey("ProductId"));

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Discount)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.PercentDiscount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Subtotal)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Total)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.InvoiceDetails)
                .WithRequired(e => e.Invoice)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(13, 2);

            modelBuilder.Entity<InvoiceStatu>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.InvoiceStatu)
                .HasForeignKey(e => e.InvoiceStatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.Products)
                .WithMany(e => e.Locations)
                .Map(m => m.ToTable("ProductsLocation").MapLeftKey("LocationId").MapRightKey("ProductId"));

            modelBuilder.Entity<Product>()
                .Property(e => e.SellPrice)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Product>()
                .Property(e => e.PurchasePrice)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.InvoiceDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Provider>()
                .HasMany(e => e.PurchaseOrders)
                .WithRequired(e => e.Provider)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Discount)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.PercentDiscount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Subtotal)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Total)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PurchaseOrderStatu>()
                .HasMany(e => e.PurchaseOrders)
                .WithRequired(e => e.PurchaseOrderStatu)
                .HasForeignKey(e => e.PurchaseOrderStatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Storage>()
                .HasMany(e => e.Locations)
                .WithRequired(e => e.Storage)
                .WillCascadeOnDelete(false);
        }
    }
}
