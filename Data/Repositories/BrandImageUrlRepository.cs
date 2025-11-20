using System;
using System.Data;
using Dapper;

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
}
