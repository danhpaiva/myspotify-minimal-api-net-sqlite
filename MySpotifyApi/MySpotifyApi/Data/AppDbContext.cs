using Microsoft.EntityFrameworkCore;

namespace MySpotifyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<MySpotifyApi.Models.Album> Album { get; set; } = default!;
        public DbSet<MySpotifyApi.Models.Artista> Artista { get; set; } = default!;
        public DbSet<MySpotifyApi.Models.Musica> Musica { get; set; } = default!;
    }
}
