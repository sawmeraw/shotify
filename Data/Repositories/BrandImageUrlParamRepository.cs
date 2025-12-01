using System;
using System.Data;
using Dapper;
using Models.DTOs;
using Shotify.Models;
using SQLitePCL;

namespace Shotify.Data;

public class BrandImageUrlParamRepository : IBrandImageUrlParamRepository
{
    private readonly IDbConnection _conn;

    public BrandImageUrlParamRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public List<BrandImageUrlParam> GetParams(int brandId)
    {
        var brandParams = _conn.Query<BrandImageUrlParam>("SELECT * FROM BrandImageUrlParams WHERE BrandId  = @BrandId ORDER BY \"Order\"", new { BrandId = brandId });
        return brandParams.ToList();
    }

    public void UpdateParams(List<BrandImageUrlParam> items)
    {
        try
        {
            const string stmt = @"
                UPDATE BrandImageUrlParams
                SET 
                    Name           = @Name,
                    Description    = @Description,
                    [Order]        = @Order,
                    FixedValue     = @FixedValue,
                    IsKeepInUrl    = @IsKeepInUrl,
                    IsAllLowerCase = @IsAllLowerCase,
                    IsAllUpperCase = @IsAllUpperCase,
                    PlaceholderInUrl = @PlaceholderInUrl
                WHERE Id = @Id;
            ";
            _conn.Execute(stmt, items);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlParamRepository.UpdateParams: {e.Message}");
            throw new Exception("Error updating url params");
        }
    }
}
