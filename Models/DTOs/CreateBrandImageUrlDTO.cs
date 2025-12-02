using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Models.DTOs;

public class CreateBrandImageUrlDTO
{
    public int Order { get; set; }
    [Url(ErrorMessage = "Please provide a valid url.")]
    public string Pattern { get; set; }
}