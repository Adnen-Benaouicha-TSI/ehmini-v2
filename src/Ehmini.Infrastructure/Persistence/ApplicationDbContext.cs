using System;
using Ehmini.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentDetail> DocumentDetails => Set<DocumentDetail>();
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Zone> Zones { get; set; } = null!;
    public DbSet<Locality> Localities { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Profession> Professions { get; set; } = null!;
    public DbSet<Prestataire> Prestataires { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Configuration existante Documents ---
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasIndex(d => d.ExternalReference).IsUnique();

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Details)
                .WithOne(dd => dd.Document)
                .HasForeignKey(dd => dd.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(d => d.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DocumentDetail>(entity =>
        {
            entity.Property(dd => dd.UnitPrice).HasPrecision(18, 2);
            entity.Property(dd => dd.LineTotal).HasPrecision(18, 2);
        });


        modelBuilder.Entity<Profession>(entity =>
        {
            entity.ToTable("proffession");
            entity.HasKey(e => e.Id).HasName("PK_proffession");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(255);
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
            entity.Property(e => e.RiskLevel).HasColumnName("risk_level").HasMaxLength(255);
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(1).IsUnicode(false);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("country");
            entity.HasKey(e => e.Id).HasName("PK_pays");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.IsoCode).HasColumnName("iso_code").HasColumnType("char(2)");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.ToTable("region");
            entity.HasKey(e => e.Id).HasName("PK_gouvernerat");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");

            entity.HasOne(r => r.Country)
                .WithMany(c => c.Regions)
                .HasForeignKey(r => r.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.ToTable("zone");
            entity.HasKey(e => e.Id).HasName("PK_delegation");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");

            entity.HasOne(z => z.Region)
                .WithMany(r => r.Zones)
                .HasForeignKey(z => z.RegionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Locality>(entity =>
        {
            entity.ToTable("locality");
            entity.HasKey(e => e.Id).HasName("PK_localite");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.ZipCode).HasColumnName("zip_code");

            entity.HasOne(l => l.Zone)
                .WithMany(z => z.Localities)
                .HasForeignKey(l => l.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("address");
            entity.HasKey(e => e.Id).HasName("PK_address_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.LocalityId).HasColumnName("locality_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");

            entity.HasOne(a => a.Locality)
                .WithMany(l => l.Addresses)
                .HasForeignKey(a => a.LocalityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Config de la relation sur ApplicationUser (AspNetUsers)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(u => u.Profession)
                .WithMany()
                .HasForeignKey(u => u.ProfessionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.Address)
                .WithMany()
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
