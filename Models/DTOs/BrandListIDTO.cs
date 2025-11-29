namespace Shotify.Models.DTOs;

public class BrandListDTO
{
    public List<BrandListItemDTO> brands { get; set; }
}

public class BrandListItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
}