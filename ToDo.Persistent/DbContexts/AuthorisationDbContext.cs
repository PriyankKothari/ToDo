using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ToDo.Persistent.DbContexts
{
    public class AuthorisationDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext
    {
        public AuthorisationDbContext(DbContextOptions<AuthorisationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            // IdentityRole
            modelBuilder.Entity<IdentityRole>().Property(ir => ir.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<IdentityRole>().Property(ir => ir.ConcurrencyStamp).IsConcurrencyToken();
            modelBuilder.Entity<IdentityRole>().Property(ir => ir.Name).HasMaxLength(256);
            modelBuilder.Entity<IdentityRole>().Property(ir => ir.NormalizedName).HasMaxLength(256);
            modelBuilder.Entity<IdentityRole>().HasKey(ir => ir.Id);
            modelBuilder.Entity<IdentityRole>().HasIndex(ir => ir.NormalizedName).IsUnique().HasName("RoleNameIndex")
                .HasFilter("[NormalizedName] IS NOT NULL");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", schema: "Identity");

            // IdentityRoleClaim
            modelBuilder.Entity<IdentityRoleClaim<string>>().Property(irc => irc.Id).ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            modelBuilder.Entity<IdentityRoleClaim<string>>().Property(irc => irc.RoleId).IsRequired();
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(irc => irc.Id);
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasIndex(irc => irc.RoleId);
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", schema: "Identity");
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasOne<IdentityRole>().WithMany()
                .HasForeignKey(irc => irc.RoleId).OnDelete(DeleteBehavior.Cascade);

            // IdentityUser
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.ConcurrencyStamp).IsConcurrencyToken();
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.Email).HasMaxLength(256);
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.NormalizedEmail).HasMaxLength(256);
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.NormalizedUserName).HasMaxLength(256);
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.UserName).HasMaxLength(256);
            modelBuilder.Entity<IdentityUser>().Property(iu => iu.Email).HasMaxLength(256);
            modelBuilder.Entity<IdentityUser>().HasKey(iu => iu.Id);
            modelBuilder.Entity<IdentityUser>().HasIndex(iu => iu.NormalizedEmail).HasName("EmailIndex");
            modelBuilder.Entity<IdentityUser>().HasIndex(iu => iu.NormalizedUserName).IsUnique()
                .HasName("UserNameIndex").HasFilter("[NormalizedUserName] IS NOT NULL");
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers", schema: "Identity");

            // IdentityUserClaim
            modelBuilder.Entity<IdentityUserClaim<string>>().Property(iuc => iuc.Id).ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            modelBuilder.Entity<IdentityUserClaim<string>>().Property(iuc => iuc.UserId).IsRequired();
            modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(iuc => iuc.Id);
            modelBuilder.Entity<IdentityUserClaim<string>>().HasIndex(iuc => iuc.UserId);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", schema: "Identity");
            modelBuilder.Entity<IdentityUserClaim<string>>().HasOne<IdentityUser>().WithMany()
                .HasForeignKey(iuc => iuc.UserId).OnDelete(DeleteBehavior.Cascade);

            // IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(iul => iul.LoginProvider).HasMaxLength(128);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(iul => iul.ProviderKey).HasMaxLength(128);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(iul => iul.UserId).IsRequired();
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(iul => iul.LoginProvider);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(iul => iul.ProviderKey);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasIndex(iul => iul.UserId);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", schema: "Identity");
            modelBuilder.Entity<IdentityUserLogin<string>>().HasOne<IdentityUser>().WithMany()
                .HasForeignKey(iul => iul.UserId).OnDelete(DeleteBehavior.Cascade);

            // IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(iur => iur.UserId);
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(iur => iur.RoleId);
            modelBuilder.Entity<IdentityUserRole<string>>().HasIndex(iur => iur.RoleId);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", schema: "Identity");
            modelBuilder.Entity<IdentityUserRole<string>>().HasOne<IdentityRole>().WithMany()
                .HasForeignKey(iur => iur.RoleId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<IdentityUserRole<string>>().HasOne<IdentityUser>().WithMany()
                .HasForeignKey(iur => iur.UserId).OnDelete(DeleteBehavior.Cascade);

            // IdentityUserToken
            modelBuilder.Entity<IdentityUserToken<string>>().Property(iut => iut.LoginProvider).HasMaxLength(128);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(iut => iut.Name).HasMaxLength(128);
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(iut => iut.UserId);
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(iut => iut.LoginProvider);
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(iut => iut.Name);
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", schema: "Identity");
            modelBuilder.Entity<IdentityUserToken<string>>().HasOne<IdentityUser>().WithMany()
                .HasForeignKey(iut => iut.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}