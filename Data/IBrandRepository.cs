using System;
using Shotify.Models;

namespace Shotify.Data;

public interface IBrandRepository
{
    public Brand GetBrandById(int id);
}
