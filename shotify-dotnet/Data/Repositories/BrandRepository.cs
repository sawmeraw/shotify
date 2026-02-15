using System;
using System.Data;
using System.Data.SqlTypes;
using Dapper;
using Shotify.Models;
using Shotify.Models.DTOs;

namespace Shotify.Data;

public class BrandRepository : IBrandRepository
{
    private readonly IDbConnection _conn;

    public BrandRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public Brand GetBrandById(int id)
    {
        try
        {
            var brand = _conn.QuerySingle<Brand>("SELECT * FROM Brands WHERE Id = @Id", new { Id = id });
            return brand;
        }
        catch (Exception e)
        {
            throw new Exception($"Caught error: {e.Message}");
        }
    }

    public List<BrandListItemDTO> GetBrandList()
    {
        try
        {
            var result = _conn.Query<BrandListItemDTO>("SELECT Id, Name FROM Brands WHERE 1 = 1");
            return result.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error: {e.Message}");
            throw new Exception("Error occurred when fetching brands.");
        }
    }

    public long CreateBrand(CreateBrandDTO payload)
    {
        try
        {
            var brand = new Brand
            {
                Name = payload.Name,
                ProductCodeCutOffChar = payload.ProductCodeCutOffChar,
                ProductCodeDelimiterChar = payload.ProductCodeDelimiterChar,
                ProductCodeDelimiterOffset = payload.ProductCodeDelimiterOffset,
                ProductCodeSliceOffset = payload.ProductCodeSliceOffset
            };

            var stmt = @"INSERT INTO Brands (Name, ProductCodeDelimiterChar, ProductCodeDelimiterOffset, ProductCodeCutOffChar, ProductCodeSliceOffset)
                VALUES (@Name, @ProductCodeDelimiterChar, @ProductCodeDelimiterOffset, @ProductCodeCutOffChar, @ProductCodeSliceOffset);
                SELECT last_insert_rowid();";

            var id = _conn.ExecuteScalar<long>(stmt, brand);
            return id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught Error: {e.Message}");
            throw new Exception("Error occurred creating the new brand.");
        }
    }

    public void UpdateBrand(int brandId, UpdateBrandDTO payload)
    {
        try
        {
            var stmt = @"
                UPDATE Brands
                SET Name = @Name,
                    ProductCodeDelimiterChar = @ProductCodeDelimiterChar,
                    ProductCodeDelimiterOffset = @ProductCodeDelimiterOffset,
                    ProductCodeCutOffChar = @ProductCodeCutOffChar,
                    ProductCodeSliceOffset = @ProductCodeSliceOffset
                WHERE Id = @Id";


            _conn.Execute(stmt, new
            {
                Id = brandId,
                payload.Name,
                payload.ProductCodeDelimiterChar,
                payload.ProductCodeDelimiterOffset,
                payload.ProductCodeCutOffChar,
                payload.ProductCodeSliceOffset
            });

        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught Error: {e.Message}");
            throw new Exception("Error occurred updating brand details.");
        }
    }

}
