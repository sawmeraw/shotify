using System;
using Microsoft.Extensions.Primitives;

namespace Shotify.Models.DTOs;

public class CreateBrandDTO
{
    public string Name { get; set; }
    public string? ProductCodeDelimiterChar { get; set; }
    public int? ProductCodeDelimiterOffset { get; set; }
    public string? ProductCodeCutOffChar { get; set; }
    public int? ProductCodeSliceOffset { get; set; }
}
