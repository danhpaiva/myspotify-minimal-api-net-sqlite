using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MySpotifyApi.Data;
using MySpotifyApi.Models;
namespace MySpotifyApi.EndPoints;

public static class MusicaEndpoints
{
    public static void MapMusicaEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Musica").WithTags(nameof(Musica));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Musica.ToListAsync();
        })
        .WithName("GetAllMusicas")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Musica>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Musica.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Musica model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetMusicaById")
        .WithOpenApi();

        group.MapGet("/musicas", async (AppDbContext context) =>
        {
            var musicasDetalhes = await context.Musica
                .Join(context.Album,
                      musica => musica.AlbumId,
                      album => album.Id,
                      (musica, album) => new { Musica = musica, Album = album })
                .Join(context.Artista,
                      joined => joined.Album.ArtistaId,
                      artista => artista.Id,
                      (joined, artista) => new MusicaDetalhe(
                          joined.Musica.Titulo,
                          artista.Nome,
                          joined.Album.Titulo
                      ))
                .ToListAsync();

            return Results.Ok(musicasDetalhes);
        })
        .WithOpenApi()
        .WithSummary("Lista todas as músicas com o nome do álbum e do artista");

        group.MapGet("/musicas/filtro", async (AppDbContext context, string? artista) =>
        {
            var query = context.Musica
                .Join(context.Album, m => m.AlbumId, a => a.Id, (m, a) => new { m, a })
                .Join(context.Artista, joined => joined.a.ArtistaId, art => art.Id, (joined, art) => new
                {
                    Musica = joined.m,
                    Album = joined.a,
                    Artista = art
                });

            if (!string.IsNullOrEmpty(artista))
            {
                query = query.Where(q => q.Artista.Nome.Contains(artista));
            }

            var musicasFiltradas = await query
                .Select(q => new MusicaDetalhe(
                    q.Musica.Titulo,
                    q.Artista.Nome,
                    q.Album.Titulo
                ))
                .ToListAsync();

            return Results.Ok(musicasFiltradas);
        })
        .WithOpenApi()
        .WithSummary("Lista músicas filtradas por nome do artista (parcial).")
        .WithDescription("Use a query string `?artista=Nome` para filtrar.");

        group.MapGet("/musicas/top", async (AppDbContext context) =>
        {
            var topMusicas = await context.Musica
                .OrderByDescending(m => m.DuracaoSegundos)
                .Take(5)
                .Join(context.Album, m => m.AlbumId, a => a.Id, (m, a) => new { m, a })
                .Join(context.Artista, joined => joined.a.ArtistaId, art => art.Id, (joined, art) => new MusicaDetalhe(
                    joined.m.Titulo,
                    art.Nome,
                    joined.a.Titulo
                ))
                .ToListAsync();

            return Results.Ok(topMusicas);
        })
        .WithOpenApi()
        .WithSummary("Retorne as 5 músicas mais longas");

        group.MapGet("/mix", async (AppDbContext context) =>
        {
            var todasMusicas = await context.Musica
                .Join(context.Album, m => m.AlbumId, a => a.Id, (m, a) => new { m, a })
                .Join(context.Artista, joined => joined.a.ArtistaId, art => art.Id, (joined, art) => new MusicaDetalhe(
                    joined.m.Titulo,
                    art.Nome,
                    joined.a.Titulo
                ))
                .ToListAsync();

            if (todasMusicas.Count == 0)
                return Results.Ok(new List<MusicaDetalhe>());

            var random = new Random();
            var mix = todasMusicas
                .OrderBy(_ => random.Next())
                .Take(10)
                .ToList();

            return Results.Ok(mix);
        })
        .WithOpenApi()
        .WithSummary("Devolve uma playlist aleatória de 10 músicas (mix).");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Musica musica, AppDbContext db) =>
        {
            var affected = await db.Musica
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, musica.Id)
                    .SetProperty(m => m.Titulo, musica.Titulo)
                    .SetProperty(m => m.DuracaoSegundos, musica.DuracaoSegundos)
                    .SetProperty(m => m.AlbumId, musica.AlbumId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateMusica")
        .WithOpenApi();

        group.MapPost("/", async (Musica musica, AppDbContext db) =>
        {
            db.Musica.Add(musica);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Musica/{musica.Id}", musica);
        })
        .WithName("CreateMusica")
        .WithOpenApi();

        group.MapPost("/musicas/batch", async (AppDbContext context, List<Musica> musicas) =>
        {
            if (musicas == null || musicas.Count == 0)
            {
                return Results.BadRequest(new { message = "Nenhuma música foi fornecida." });
            }

            var albumIds = musicas.Select(m => m.AlbumId).Distinct();
            var albunsExistentes = await context.Album
                .Where(a => albumIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            var albunsNaoEncontrados = albumIds.Except(albunsExistentes).ToList();

            if (albunsNaoEncontrados.Any())
            {
                return Results.NotFound(new { message = $"Um ou mais AlbumIds não foram encontrados: {string.Join(", ", albunsNaoEncontrados)}" });
            }

            context.Musica.AddRange(musicas);
            await context.SaveChangesAsync();
            return Results.Created("/musicas", new { count = musicas.Count });
        })
            .WithOpenApi()
            .WithSummary("Cadastra múltiplas músicas em uma única requisição.");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Musica
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteMusica")
        .WithOpenApi();
    }
}
