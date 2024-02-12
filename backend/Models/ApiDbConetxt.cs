using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<TagNote>? TagsNotes { get; set; }
        public DbSet<Note>? Notes { get; set; }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateDate = DateTime.Now;
                        entry.Entity.EditDate = entry.Entity.CreateDate;
                        break;
                    case EntityState.Modified:
                        entry.Entity.EditDate = DateTime.Now;
                        break;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            {
                if (!optionsBuilder.IsConfigured)
                {
                    string connectionString = string.Empty;
                    try
                    {
                        // Create an instance of StreamReader to read from a file.
                        // The using statement also closes the StreamReader.

                        using (StreamReader sr = new StreamReader("StringConnection.txt"))
                        {

                            // Read and display lines from the file until the end of
                            // the file is reached.
                            connectionString = sr.ReadToEnd();
                        }
                    }
                    catch (Exception e)
                    {
                        // Let the user know what went wrong.
                        Console.WriteLine("The file could not be read:");
                        Console.WriteLine(e.Message);
                    }

                    optionsBuilder.UseSqlServer(connectionString)
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
                    .EnableSensitiveDataLogging();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasMany(n => n.Notes)
                .WithOne(u => u.User)
                .HasPrincipalKey(u => u.Id);

            modelBuilder.Entity<Note>()
                .HasMany(a => a.Tags)
                .WithOne(a => a.Note)
                .HasPrincipalKey(a=>a.Id)
                .HasForeignKey(a=>a.NoteId);
        }
    }
}