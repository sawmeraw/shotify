using System;
using Shotify.Models;
using Shotify.Models.DTOs;

namespace Shotify.Data;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _db;

    public BrandRepository(AppDbContext db)
    {
        _db = db;
    }

    public Brand GetBrandById(int id)
    {
        try
        {
            var brand = _db.Brands.FirstOrDefault(b => b.Id == id);
            if (brand == null)
                throw new Exception($"Brand with Id {id} not found.");
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
            return _db.Brands
                .Select(b => new BrandListItemDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsDeleted = b.IsDeleted
                })
                .ToList();
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

            _db.Brands.Add(brand);
            _db.SaveChanges();
            return brand.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught Error: {e.Message}");
            throw new Exception("Error occurred creating the new brand.");
        }
    }

    public void SetDeleted(int brandId, bool isDeleted)
    {
        var brand = _db.Brands.FirstOrDefault(b => b.Id == brandId);
        if (brand != null)
        {
            brand.IsDeleted = isDeleted;
            _db.SaveChanges();
        }
    }

    public void UpdateBrand(int brandId, UpdateBrandDTO payload)
    {
        try
        {
            var brand = _db.Brands.FirstOrDefault(b => b.Id == brandId);
            if (brand == null)
                throw new Exception($"Brand with Id {brandId} not found.");

            brand.Name = payload.Name;
            brand.ProductCodeDelimiterChar = payload.ProductCodeDelimiterChar;
            brand.ProductCodeDelimiterOffset = payload.ProductCodeDelimiterOffset;
            brand.ProductCodeCutOffChar = payload.ProductCodeCutOffChar;
            brand.ProductCodeSliceOffset = payload.ProductCodeSliceOffset;

            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught Error: {e.Message}");
            throw new Exception("Error occurred updating brand details.");
        }
    }
}
