using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MySpotifyApi.Data;
using MySpotifyApi.Models;
namespace MySpotifyApi.EndPoints;

public static class ArtistaEndpoints
{
    public static void MapArtistaEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Artista").WithTags(nameof(Artista));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Artista.ToListAsync();
        })
        .WithName("GetAllArtista")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Artista>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Artista.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Artista model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetArtistaById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Artista artista, AppDbContext db) =>
        {
            var affected = await db.Artista
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, artista.Id)
                    .SetProperty(m => m.Nome, artista.Nome)
                    .SetProperty(m => m.Nacionalidade, artista.Nacionalidade)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateArtista")
        .WithOpenApi();

        group.MapPost("/", async (Artista artista, AppDbContext db) =>
        {
            db.Artista.Add(artista);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Artista/{artista.Id}",artista);
        })
        .WithName("CreateArtista")
        .WithOpenApi();

        group.MapPost("/artistas/batch", async (AppDbContext context, List<Artista> artistas) =>
        {
            context.Artista.AddRange(artistas);
            await context.SaveChangesAsync();
            return Results.Created($"/artistas", artistas);
        });

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Artista
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteArtista")
        .WithOpenApi();
    }
}
