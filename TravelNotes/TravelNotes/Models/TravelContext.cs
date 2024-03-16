using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TravelNotes.Models;

public partial class TravelContext : IdentityDbContext<IdentityUser>
{
    public TravelContext(DbContextOptions<TravelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArticleTagList> ArticleTagList { get; set; }

    public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }

    public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

    public virtual DbSet<Itinerary> Itinerary { get; set; }

    public virtual DbSet<LookBack> LookBack { get; set; }

    public virtual DbSet<Spots> Spots { get; set; }

    public virtual DbSet<StatisticsCity> StatisticsCity { get; set; }

    public virtual DbSet<TagList> TagList { get; set; }

    public virtual DbSet<TripPlanning> TripPlanning { get; set; }

    public virtual DbSet<UniqueCity> UniqueCity { get; set; }

    public virtual DbSet<album> album { get; set; }

    public virtual DbSet<article> article { get; set; }

    public virtual DbSet<articletags> articletags { get; set; }

    public virtual DbSet<messageBoard> messageBoard { get; set; }

    public virtual DbSet<photo> photo { get; set; }

    public virtual DbSet<users> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleTagList>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__ArticleT__397E2BC31D302524");

            entity.Property(e => e.TagClass)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TagName)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AspNetRoleClaims>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetUserClaims>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.Discriminator).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUsers>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Itinerary");

            entity.Property(e => e.Class1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class3)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DescriptionDetail)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.GeoHash)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Keyword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OpenTime)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ParkingGeoHash)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ParkingInfo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ParkingLat)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.ParkingLon)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PositionLat)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.PositionLon)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ScenicSpotID)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScenicSpotName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TicketInfo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TravelInfo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e._Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e._Description)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e._level)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.city)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<LookBack>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Photo).WithMany()
                .HasForeignKey(d => d.PhotoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LookBack_photo");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LookBack_users");
        });

        modelBuilder.Entity<Spots>(entity =>
        {
            entity.HasKey(e => e.ScenicSpotID).HasName("PK__Spots__C0F055A6C7566260");

            entity.Property(e => e.ScenicSpotID)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.City)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Class1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class3)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DescriptionDetail)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.GeoHash)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Keyword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OpenTime)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ParkingGeoHash)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ParkingInfo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ParkingLat)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.ParkingLon)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureDescription3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PictureUrl3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PositionLat)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.PositionLon)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ScenicSpotName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TicketInfo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TravelInfo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e._Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e._Description)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e._level)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StatisticsCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StatisticsCity");

            entity.Property(e => e.city)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<TagList>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__TagList__397E2BC325A3E238");

            entity.Property(e => e.TagClass)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.TagName)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TripPlanning>(entity =>
        {
            entity.HasKey(e => e.TripId);

            entity.Property(e => e.ScenicSpotID)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.ScenicSpot).WithMany(p => p.TripPlanning)
                .HasForeignKey(d => d.ScenicSpotID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripPlanning_Spots");

            entity.HasOne(d => d.User).WithMany(p => p.TripPlanning)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripPlanning_users");
        });

        modelBuilder.Entity<UniqueCity>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.City)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__album__97B4BE379A4A0F1D");

            entity.Property(e => e.AlbumName).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.album)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_album_AlbumId");
        });

        modelBuilder.Entity<article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__article__9C6270E824592651");

            entity.Property(e => e.ArticleState)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Contents).HasMaxLength(2500);
            entity.Property(e => e.Images)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PublishTime).HasColumnType("datetime");
            entity.Property(e => e.Subtitle).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(20);
            entity.Property(e => e.TravelTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.article)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_UserId");
        });

        modelBuilder.Entity<articletags>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__articlet__397E2BC38799DE31");

            entity.Property(e => e.LabelDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LabelName)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Article).WithMany(p => p.articletags)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("fk_tags_ArticleId");
        });

        modelBuilder.Entity<messageBoard>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__messageB__C87C0C9C4A7A0112");

            entity.Property(e => e.Contents).HasMaxLength(2500);
            entity.Property(e => e.MessageTime).HasColumnType("datetime");

            entity.HasOne(d => d.Article).WithMany(p => p.messageBoard)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_messageBoard_ArticleId");

            entity.HasOne(d => d.User).WithMany(p => p.messageBoard)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_messageBoard_UserId");
        });

        modelBuilder.Entity<photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__photo__21B7B5E2D9618FE2");

            entity.Property(e => e.PhotoDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhotoTitle)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Album).WithMany(p => p.photo)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("fk_photo_AlbumId");

            entity.HasOne(d => d.User).WithMany(p => p.photo)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_photo_users");
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__1788CC4C21EE8A4B");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Headshot)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Interest).HasMaxLength(10);
            entity.Property(e => e.Introduction).HasMaxLength(150);
            entity.Property(e => e.Mail)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.Nickname).HasMaxLength(32);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Pwd)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.SuperUser)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(25);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
