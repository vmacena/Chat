using Chat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Core.Persistence;

public partial class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public virtual DbSet<Auth> Auths { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Uploadfile> Uploadfiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5435;Database=postgres;Username=postgres;Password=root;"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auth>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("auth_pkey");

            entity.Property(e => e.Userid).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Auth).HasConstraintName("fk_auth_user");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => new { e.Userid, e.Contactid }).HasName("contacts_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.Status).HasDefaultValueSql("'pending'::text");

            entity
                .HasOne(d => d.ContactNavigation)
                .WithMany(p => p.ContactContactNavigations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Contacts_Users_ContactId");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.ContactUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Contacts_Users_UserId");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Messages");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity
                .HasOne(d => d.Receiver)
                .WithMany(p => p.MessageReceivers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Messages_Users_ReceiverId");

            entity
                .HasOne(d => d.Sender)
                .WithMany(p => p.MessageSenders)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Messages_Users_SenderId");
        });

        modelBuilder.Entity<Uploadfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UploadFiles");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Uploadedat).HasDefaultValueSql("now()");

            entity
                .HasOne(d => d.Message)
                .WithMany(p => p.Uploadfiles)
                .HasConstraintName("FK_UploadFiles_Messages_MessageId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Createdat).HasDefaultValueSql("now()");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
