using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class BrandImageUrlReadDTO
{
    public int Id { get; set; }
    public int? Order { get; set; }
    [Required(ErrorMessage ="Required")]
    public string Pattern { get; set; }

}