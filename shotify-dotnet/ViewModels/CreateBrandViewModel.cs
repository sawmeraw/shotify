using System.ComponentModel.DataAnnotations;

namespace ViewModels;

public class CreateBrandViewModel
{
    [Required(ErrorMessage = "Brand name is required.")]
    public string Name { get; set; }

    public string? ProductCodeDelimiterChar { get; set; }

    public int? ProductCodeDelimiterOffset { get; set; }

    public string? ProductCodeCutOffChar { get; set; }

    public int? ProductCodeSliceOffset { get; set; }
}
