using System;
namespace Application.Extensions;

public class CustomExtensions
{
    public string GenerateUniqueCustomerCode()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = new Random().Next(1000, 9999);
        return $"CUST{datePart}{randomPart}";
    }
}
