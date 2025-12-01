using System;
using System.Data;
using Dapper;
using Shotify.Models;

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


}
