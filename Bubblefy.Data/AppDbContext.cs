using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Data;

public class AppDbContext : DbContext
{
    public DbSet<Faixa> Faixas => Set<Faixa>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    private readonly string connectionString =
        "server=localhost;port=3305;database=Bubblefy;uid=root;pwd=1234";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faixa>()
            .HasMany(f => f.Playlists)
            .WithMany(p => p.Faixas)
            .UsingEntity(j => j.ToTable("FaixaPlaylist"));

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Nome)
            .HasMaxLength(50);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Nome)
            .IsUnique();
    }
}