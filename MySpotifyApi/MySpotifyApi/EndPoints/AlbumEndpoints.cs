using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MySpotifyApi.Data;
using MySpotifyApi.Models;
namespace MySpotifyApi.EndPoints;

public static class AlbumEndpoints
{
    public static void MapAlbumEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Album").WithTags(nameof(Album));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Album.ToListAsync();
        })
        .WithName("GetAllAlbums")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Album>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Album.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Album model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAlbumById")
        .WithOpenApi();

        group.MapGet("/albuns", async (AppDbContext context) =>
        {
            var albunsDetalhes = await context.Album
                .Join(context.Artista,
                      album => album.ArtistaId,
                      artista => artista.Id,
                      (album, artista) => new
                      {
                          album.Id,
                          album.Titulo,
                          album.AnoLancamento,
                          ArtistaNome = artista.Nome
                      })
                .ToListAsync();

            return Results.Ok(albunsDetalhes);
        })
        .WithOpenApi()
        .WithSummary("Lista todos os álbuns com seus artistas"); ;

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Album album, AppDbContext db) =>
        {
            var affected = await db.Album
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, album.Id)
                    .SetProperty(m => m.Titulo, album.Titulo)
                    .SetProperty(m => m.AnoLancamento, album.AnoLancamento)
                    .SetProperty(m => m.ArtistaId, album.ArtistaId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateAlbum")
        .WithOpenApi();

        group.MapPost("/", async (Album album, AppDbContext db) =>
        {
            db.Album.Add(album);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Album/{album.Id}", album);
        })
        .WithName("CreateAlbum")
        .WithOpenApi();

        group.MapPost("/albuns/batch", async (AppDbContext context, List<Album> albuns) =>
        {
            if (albuns == null || albuns.Count == 0)
            {
                return Results.BadRequest(new { message = "Nenhum álbum foi fornecido." });
            }

            var artistasIds = albuns.Select(a => a.ArtistaId).Distinct();
            var artistasExistentes = await context.Artista
                .Where(a => artistasIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            var artistasNaoEncontrados = artistasIds.Except(artistasExistentes).ToList();

            if (artistasNaoEncontrados.Any())
            {
                return Results.NotFound(new { message = $"Um ou mais ArtistasIds não foram encontrados: {string.Join(", ", artistasNaoEncontrados)}" });
            }

            context.Album.AddRange(albuns);
            await context.SaveChangesAsync();
            return Results.Created("/albuns", new { count = albuns.Count });
        })
        .WithOpenApi()
        .WithSummary("Cadastra múltiplos álbuns em uma única requisição.");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Album
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteAlbum")
        .WithOpenApi();
    }
}
