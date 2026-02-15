using System;

namespace Shotify.Models;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; }
    //done
    public string? ProductCodeDelimiterChar { get; set; }

    public int? ProductCodeDelimiterOffset { get; set; }
    //done
    public string? ProductCodeCutOffChar { get; set; }
    public int? ProductCodeSliceOffset { get; set; }

    public DateTime CreatedAt { get; set; }

}
