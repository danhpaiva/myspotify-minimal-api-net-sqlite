using System.ComponentModel.DataAnnotations;

namespace MySpotifyApi.Models;

public class Artista
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "O campo Nacionalidade é obrigatório.")]
    public string Nacionalidade { get; set; }
}
