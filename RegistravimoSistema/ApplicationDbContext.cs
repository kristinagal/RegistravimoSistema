using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RegistravimoSistema.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace RegistravimoSistema
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.Role)
                      .IsRequired();

                entity.HasMany(u => u.Persons)
                      .WithOne(p => p.User)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Person 
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Vardas)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Pavarde)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.AsmensKodas)
                      .IsRequired()
                      .HasMaxLength(11)
                      .HasComment("Format: 1YYMMDDXXXX");

                entity.Property(p => p.TelefonoNumeris)
                      .IsRequired()
                      .HasMaxLength(12)
                      .HasComment("Starts with '8' or '+370'");

                entity.Property(p => p.ElPastas)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.ProfilioNuotrauka)
                      .IsRequired();

                // One-to-One Relationship with Address
                entity.HasOne(p => p.Address)
                      .WithOne(a => a.Person)
                      .HasForeignKey<Address>(a => a.PersonId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Address 
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Miestas)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(a => a.Gatve)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.NamoNumeris)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(a => a.ButoNumeris)
                      .HasMaxLength(20);
            });

            // Unique Constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.AsmensKodas)
                .IsUnique();

            // Seed Initial Admin Data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            var adminUserId = Guid.NewGuid();
            var adminPersonId = Guid.NewGuid();
            var adminAddressId = Guid.NewGuid();

            var passwordSalt = GenerateSalt();
            var passwordHash = GeneratePasswordHash("Admin@123", passwordSalt);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminUserId,
                Username = "Admin",
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                Role = "Admin"
            });

            modelBuilder.Entity<Person>().HasData(new Person
            {
                Id = adminPersonId,
                UserId = adminUserId,
                Vardas = "Admin",
                Pavarde = "User",
                AsmensKodas = "19901010001",
                TelefonoNumeris = "+37060012345",
                ElPastas = "admin@example.com",
                ProfilioNuotrauka = Array.Empty<byte>()
            });

            modelBuilder.Entity<Address>().HasData(new Address
            {
                Id = adminAddressId,
                PersonId = adminPersonId,
                Miestas = "Vilnius",
                Gatve = "Gedimino pr.",
                NamoNumeris = "1",
                ButoNumeris = "101"
            });
        }


        private byte[] GenerateSalt()
        {
            var salt = new byte[16]; // 16 bytes = 128 bits
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        private byte[] GeneratePasswordHash(string password, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
