using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using BitcoinRhExplorer.Entities.Blocks;
using BitcoinRhExplorer.Entities.Stats;
using BitcoinRhExplorer.Entities.Users;

namespace BitcoinRhExplorer.Server
{
    public class BitcoinRhExplorerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AddressFromTo> AddressesFromTo { get; set; }

        public DbSet<DiffStat> DiffStats { get; set; }
        public DbSet<RichStat> RichStats { get; set; }

        /// <summary>
        /// Whether the database has been initialized.
        /// </summary>
        public static bool DatabaseIsInitialized { get; private set; }

        public void Initialize()
        {
            if (!DatabaseIsInitialized)
            {
                // Initialize the database and migrate it to the latest version.
                Database.SetInitializer(
                    new MigrateDatabaseToLatestVersion<BitcoinRhExplorerDbContext, BitcoinRhExplorerDbMigrationsConfiguration>());
                Database.Initialize(true);

                DatabaseIsInitialized = true;
            }
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            // Prevents from unwanted or accidental cascade deletes. Soft-delete is used mostly anyway.
            mb.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Roles.
            mb.Entity<Role>().Property(r => r.Description).HasMaxLength(250);
            mb.Entity<Role>().Property(r => r.Name).HasMaxLength(100);

            //Users.
            mb.Entity<User>().Property(u => u.Email).HasMaxLength(100);
            mb.Entity<User>().Property(u => u.Email).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_Email") {IsUnique = false}
            }));
            mb.Entity<User>().Property(u => u.UserName).HasMaxLength(255);
            mb.Entity<User>().Property(u => u.UserName).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_UserName") {IsUnique = true}
            }));
            mb.Entity<User>().Property(u => u.Language).HasMaxLength(2);
            mb.Entity<User>().Property(u => u.FirstName).HasMaxLength(100);
            mb.Entity<User>().Property(u => u.LastName).HasMaxLength(100);
            mb.Entity<User>().Property(u => u.Phone).HasMaxLength(20);
            mb.Entity<User>().Ignore(u => u.ConfirmPassword);
            mb.Entity<User>().HasRequired(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);

            //Block.
            mb.Entity<Block>().Property(u => u.Hash).HasMaxLength(256);
            //mb.Entity<Block>().Property(u => u.Hash).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            //{
            //    new IndexAttribute("IX_Hash") {IsUnique = true}
            //}));
            mb.Entity<Block>().Property(u => u.HashIndex).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_HashIndex") {IsUnique = false}
            }));
            mb.Entity<Block>().Property(u => u.Height).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_Height") {IsUnique = true}
            }));

            //Transaction.
            mb.Entity<Transaction>().HasRequired(u => u.Block).WithMany().HasForeignKey(u => u.BlockId);
            mb.Entity<Transaction>().Property(u => u.Hash).HasMaxLength(256);
            //mb.Entity<Transaction>().Property(u => u.Hash).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            //{
            //    new IndexAttribute("IX_Hash") {IsUnique = true}
            //}));
            mb.Entity<Transaction>().Property(u => u.HashIndex).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_HashIndex") {IsUnique = false}
            }));

            //AddressFromTo.
            mb.Entity<AddressFromTo>().HasRequired(u => u.Block).WithMany().HasForeignKey(u => u.BlockId);
            mb.Entity<AddressFromTo>().Property(u => u.Address).HasMaxLength(128);
            mb.Entity<AddressFromTo>().Property(u => u.AddressIndex).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_AddressIndex") {IsUnique = false}
            }));

            //RichStat.
            mb.Entity<RichStat>().Property(u => u.Address).HasMaxLength(128);
            mb.Entity<RichStat>().Property(u => u.AddressIndex).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("IX_AddressIndex") {IsUnique = false}
            }));

        }
    }

    internal class BitcoinRhExplorerDbMigrationsConfiguration : DbMigrationsConfiguration<BitcoinRhExplorerDbContext>
    {
        public BitcoinRhExplorerDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(BitcoinRhExplorerDbContext context)
        {
            base.Seed(context);

            var actualDate = DateTime.UtcNow;

            if (!context.Roles.Any())
            {
                //Roles.
                List<Role> defaultRoles = new List<Role>
                {
                    new Role
                    {
                        Id = 1,
                        Name = "SuperAdmin",
                        Description = "",
                        CreatedUtc = actualDate,
                        IsDeleted = false,
                        UpdatedUtc = actualDate
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Admin",
                        Description = "",
                        CreatedUtc = actualDate,
                        IsDeleted = false,
                        UpdatedUtc = actualDate
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Moderator",
                        Description = "",
                        CreatedUtc = actualDate,
                        IsDeleted = false,
                        UpdatedUtc = actualDate
                    },
                    new Role
                    {
                        Id = 4,
                        Name = "User",
                        Description = "",
                        CreatedUtc = actualDate,
                        IsDeleted = false,
                        UpdatedUtc = actualDate
                    }
                };
                foreach (var role in defaultRoles)
                {
                    context.Roles.Add(role);
                }
                context.SaveChanges();

                //Users.
                var superAdmin = new User
                {
                    Id = 1,
                    Email = "master@xrhodium.org",
                    UserName = "master",
                    RoleId = 1,
                    FirstName = "Master",
                    LastName = "BTR",
                    Language = "en",
                    Password = "3DD3A7445FEFA3269F11F61B525EF0EB39E44C86", //14141414
                    CreatedUtc = actualDate,
                    IsDeleted = false,
                    UpdatedUtc = actualDate
                };
                context.Users.Add(superAdmin);
                context.SaveChanges();

                //Diff.
                var diff = new DiffStat
                {
                    Id = 1,
                    Height = 0,
                    Diff = 0.000244137132,
                    CreatedUtc = actualDate,
                    IsDeleted = false,
                    UpdatedUtc = actualDate
                };
                context.DiffStats.Add(diff);
                context.SaveChanges();
            }
        }
    }
}
