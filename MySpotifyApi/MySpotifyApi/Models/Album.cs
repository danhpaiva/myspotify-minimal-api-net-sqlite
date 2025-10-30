using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySpotifyApi.Models;

public class Album
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "O campo Titulo é obrigatório.")]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O campo AnoLancamento é obrigatório.")]
    public int AnoLancamento { get; set; }
    [ForeignKey("Artista")]
    public int ArtistaId { get; set; }
}
