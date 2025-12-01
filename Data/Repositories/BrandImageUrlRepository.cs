using System;
using System.Data;
using Dapper;
using Models.DTOs;

namespace Shotify.Services;

public class BrandImageUrlRepository : IBrandImageUrlRepository
{
    private readonly IDbConnection _conn;

    public BrandImageUrlRepository(IDbConnection conn)
    {
        _conn = conn;
    }
    public List<string>? GetPatterns(int brandId)
    {
        var patterns = _conn.Query<string>(
            "SELECT Pattern FROM BrandImageUrls WHERE BrandId = @BrandId ORDER BY \"Order\"",
            new { BrandId = brandId }
        );

        if (patterns == null)
        {
            return null;
        }

        return patterns.ToList();
    }

    public List<BrandImageUrlDTO>? GetBrandImageUrlParams(int brandId)
    {
        var patterns = _conn.Query<BrandImageUrlDTO>(
            "SELECT Id, \"Order\", Pattern FROM BrandImageUrls WHERE BrandId = @BrandId ORDER BY \"Order\"",
            new { BrandId = brandId }
        );

        if (patterns == null)
        {
            return null;
        }

        return patterns.ToList();
    }

    public void UpdateBrandImageUrls(List<BrandImageUrlDTO> items)
    {
        try
        {
            var stmt = @"UPDATE BrandImageUrls SET ""Order"" = @Order, Pattern = @Pattern WHERE Id = @Id";
            foreach (var item in items)
            {
                if (item != null)
                {
                    _conn.Execute(stmt, item);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlRepository.UpdateBrandImageUrls: {e.Message}");
            throw new Exception("Error updating brand image urls");
        }
    }
}
