using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;

public partial class TravelContext : DbContext
{
    public TravelContext()
    {
    }

    public TravelContext(DbContextOptions<TravelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<LabelKeyword> LabelKeywords { get; set; }

    public virtual DbSet<LabelManage> LabelManages { get; set; }

    public virtual DbSet<MessageBoard> MessageBoards { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<RecommandBackup> RecommandBackups { get; set; }

    public virtual DbSet<Recommend> Recommends { get; set; }

    public virtual DbSet<Spot> Spots { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Travel;Integrated Security=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__album__97B4BE37EE594B80");

            entity.ToTable("album");

            entity.Property(e => e.AlbumName).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Albums)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_album_AlbumId");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__article__9C6270E81ECC6DC6");

            entity.ToTable("article");

            entity.Property(e => e.ArticleState)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Contents).HasMaxLength(2500);
            entity.Property(e => e.Images)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.PublishTime).HasColumnType("datetime");
            entity.Property(e => e.Subtitle).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(20);
            entity.Property(e => e.TravelTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_UserId");
        });

        modelBuilder.Entity<LabelKeyword>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LabelKeyword");

            entity.Property(e => e.Result)
                .HasMaxLength(400)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LabelManage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LabelManage");

            entity.Property(e => e.City)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("city");
            entity.Property(e => e.Class1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class3)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Keyword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Level)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("_level");
            entity.Property(e => e.ScenicSpotName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MessageBoard>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__messageB__C87C0C9CAC9D0830");

            entity.ToTable("messageBoard");

            entity.Property(e => e.Contents).HasMaxLength(2500);

            entity.HasOne(d => d.Article).WithMany(p => p.MessageBoards)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_messageBoard_ArticleId");

            entity.HasOne(d => d.User).WithMany(p => p.MessageBoards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_messageBoard_UserId");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__photo__21B7B5E2D8F188A4");

            entity.ToTable("photo");

            entity.Property(e => e.Haedshot)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PhotoDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhotoTitle)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Album).WithMany(p => p.Photos)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("fk_photo_AlbumId");
        });

        modelBuilder.Entity<RecommandBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("recommandBackup");

            entity.Property(e => e.Age)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("age");
            entity.Property(e => e.Gender)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.Interest1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("interest1");
            entity.Property(e => e.Interest2)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("interest2");
            entity.Property(e => e.Interest3)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("interest3");
            entity.Property(e => e.LikeLocation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("likeLocation");
            entity.Property(e => e.LikeWeather)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("likeWeather");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("userId");
        });

        modelBuilder.Entity<Recommend>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__recommen__397E2BC3E27ACBAB");

            entity.ToTable("recommend");

            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Interest)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Interest2)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Interest3)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Weather)
                .HasMaxLength(4)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Spot>(entity =>
        {
            entity.HasKey(e => e.ScenicSpotId).HasName("PK__Spots__C0F055A6A85AF331");

            entity.Property(e => e.ScenicSpotId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ScenicSpotID");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("_Address");
            entity.Property(e => e.City)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("city");
            entity.Property(e => e.Class1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Class3)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("_Description");
            entity.Property(e => e.DescriptionDetail)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.GeoHash)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Keyword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Level)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("_level");
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
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__tags__397E2BC31E8C2234");

            entity.ToTable("tags");

            entity.Property(e => e.LabelDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LabelName)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Article).WithMany(p => p.Tags)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("fk_tags_ArticleId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__1788CC4C01D43B6B");

            entity.ToTable("users");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Haedshot)
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
