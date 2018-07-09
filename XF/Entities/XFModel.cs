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
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientNote> ClientNotes { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<InvoiceStatu> InvoiceStatus { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentOption> PaymentOptions { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrderStatu> PurchaseOrderStatus { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
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

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.ClientNotes)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.ClientNotes)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

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
                .Property(e => e.Tax)
                .HasPrecision(12, 5);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Total)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.SNAP)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.DeliveryFee)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.InstalationFee)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Refund)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.InvoiceDetails)
                .WithRequired(e => e.Invoice)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.Invoice)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.PurchaseOrders)
                .WithMany(e => e.Invoices)
                .Map(m => m.ToTable("InvoiceOrders").MapLeftKey("InvoiceId").MapRightKey("PurchaseOrderId"));

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(13, 2);

            modelBuilder.Entity<InvoiceStatu>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.InvoiceStatu)
                .HasForeignKey(e => e.InvoiceStatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.Stocks)
                .WithRequired(e => e.Location)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                .Property(e => e.Amount)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Payment>()
                .Property(e => e.Balance)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PaymentOption>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.PaymentOption)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentType>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.PaymentType)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<Product>()
                .HasMany(e => e.PurchaseOrderDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Stocks)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Provider>()
                .HasMany(e => e.Products)
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
                .Property(e => e.Tax)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Total)
                .HasPrecision(13, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(e => e.PurchaseOrderDetails)
                .WithRequired(e => e.PurchaseOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.UnitPrice)
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
