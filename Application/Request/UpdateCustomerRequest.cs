using System.ComponentModel.DataAnnotations;

namespace Application.Request;

public class UpdateCustomerRequest
{
    [MaxLength(50)]
    public string? CustomerCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string? CustomerName { get; set; }

    public string? CustomerAddress { get; set; }

    [Required]
    public int? ModifiedBy { get; set; }
}
