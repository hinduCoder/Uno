using System.Data.Entity;

namespace WebClient.DataAccess
{
    public partial class UnoDbContext : DbContext
    {
        public UnoDbContext()
#if DEBUG
            : base("name=UnoDbContextLocal")
#else
            : base("name=UnoDbContext")
#endif
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<UnoDbContext>());
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(u => u.Email);
            
        }
    }
}
