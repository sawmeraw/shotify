using System;
using System.Data;
using Dapper;
using Shotify.Models;

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
        var brand = _conn.QuerySingle<Brand>("SELECT * FROM Brands WHERE Id = @Id", new { Id = id });
        return brand;
    }
}
