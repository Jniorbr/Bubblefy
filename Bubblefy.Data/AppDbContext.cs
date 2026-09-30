using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Data;

public class AppDbContext : DbContext
{
    public DbSet<Faixa> Faixas => Set<Faixa>();
    public DbSet<Playlist> Playlists => Set<Playlist>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=bubblefy.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faixa>()
            .HasMany(f => f.Playlists)
            .WithMany(p => p.Faixas)
            .UsingEntity(j => j.ToTable("FaixaPlaylist"));
    }
}