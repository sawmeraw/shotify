using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class UpdateBrandImageUrlDTO
{
    public int Id { get; set; }
    public int? Order { get; set; }
    [Required]
    [Url(ErrorMessage = "Pattern needs to be an url.")]
    public string Pattern { get; set; }
}