# 🎧 MySpotifyApi  

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-8.0-green?style=flat-square)](https://learn.microsoft.com/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

> Uma Minimal API inspirada no Spotify — simples, rápida e moderna.  
> Desenvolvida em **.NET 8** com **Entity Framework Core + SQLite**, pronta para expansão e integração com aplicações reais.

---

## 🧠 Visão Geral  

**MySpotifyApi** é uma API minimalista que simula operações básicas do Spotify, permitindo gerenciar artistas, álbuns e músicas.  
Ela foi criada para estudos e demonstrações de **boas práticas com Minimal APIs** e **EF Core**.  

---

## ⚙️ Tecnologias Utilizadas  

- **.NET 8.0** — Minimal API  
- **Entity Framework Core (EF Core)** — ORM para persistência  
- **SQLite** — Banco de dados leve e rápido  
- **Swagger / OpenAPI** — Documentação interativa  
- **Docker** — Empacotamento e execução simplificada  

---

## 📂 Estrutura do Projeto  

```bash
MySpotifyApi/
├── Data/
│   └── AppDbContext.cs
├── EndPoints/
│   ├── AlbumEndpoints.cs
│   ├── ArtistaEndpoints.cs
│   └── MusicaEndpoints.cs
├── Models/
│   ├── Album.cs
│   ├── Artista.cs
│   └── Musica.cs
├── Migrations/
├── appsettings.json
├── Program.cs
└── Dockerfile
