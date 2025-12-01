
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.DTOs;
using Shotify.Models;

namespace ViewModels;

public class EditBrandViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    //done
    public string? ProductCodeDelimiterChar { get; set; }

    public int? ProductCodeDelimiterOffset { get; set; }
    //done
    public string? ProductCodeCutOffChar { get; set; }
    public int? ProductCodeSliceOffset { get; set; }
    public List<BrandImageUrlDTO>? ImageUrls { get; set; }
    public List<BrandImageUrlParam>? UrlParams { get; set; }

}