using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MySpotifyApi.Models;

public class Musica
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "O campo Titulo é obrigatório.")]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O campo AnoLancamento é obrigatório.")]
    public double DuracaoSegundos { get; set; }
    [ForeignKey("Album")]
    public int AlbumId { get; set; }
}
