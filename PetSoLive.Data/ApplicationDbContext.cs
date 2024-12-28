using Microsoft.EntityFrameworkCore;
using PetSoLive.Core.Entities;

namespace PetSoLive.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<Assistance> Assistances { get; set; }
        public DbSet<PetOwner> PetOwners { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }

        // Yeni eklemeler
        public DbSet<LostPetAd> LostPetAds { get; set; }
        public DbSet<HelpRequest> HelpRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Adoption - Pet ilişkisi
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            // PetOwner - Pet ve User ilişkisi (Many-to-Many)
            modelBuilder.Entity<PetOwner>()
                .HasKey(po => new { po.PetId, po.UserId });

            modelBuilder.Entity<PetOwner>()
                .HasOne(po => po.Pet)
                .WithMany(p => p.PetOwners)
                .HasForeignKey(po => po.PetId);

            modelBuilder.Entity<PetOwner>()
                .HasOne(po => po.User)
                .WithMany(u => u.PetOwners)
                .HasForeignKey(po => po.UserId);

            // AdoptionRequest - Pet ve User ilişkisi
            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(ar => ar.Pet)
                .WithMany(p => p.AdoptionRequests)
                .HasForeignKey(ar => ar.PetId);

            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(ar => ar.UserId);

            // LostPetAd - User ilişkisi
            modelBuilder.Entity<LostPetAd>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId);

            // HelpRequest - User ilişkisi

            modelBuilder.Entity<HelpRequest>()
                .HasOne(h => h.User)
                .WithMany(u => u.HelpRequests)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Silme davranışını belirleyin, isteğe bağlı

            // PetOwner ve Adoption gibi tabloların isimlerini özelleştiriyoruz
            modelBuilder.Entity<PetOwner>().ToTable("PetOwners");
            modelBuilder.Entity<Adoption>().ToTable("Adoptions");
            modelBuilder.Entity<AdoptionRequest>().ToTable("AdoptionRequests");
            modelBuilder.Entity<LostPetAd>().ToTable("LostPetAds");
            modelBuilder.Entity<HelpRequest>().ToTable("HelpRequests");
        }
    }
}
