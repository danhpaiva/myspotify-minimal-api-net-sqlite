using Microsoft.EntityFrameworkCore;
using MySpotifyApi.Data;
using MySpotifyApi.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddDbContext<AppDbContext>(options =>
    options
    .UseSqlite(builder
    .Configuration
    .GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapAlbumEndpoints();

app.MapArtistaEndpoints();

app.MapMusicaEndpoints();

app.Run();
