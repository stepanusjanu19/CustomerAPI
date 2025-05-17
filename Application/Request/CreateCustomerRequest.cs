using System.ComponentModel.DataAnnotations;

namespace Application.Request;

public class CreateCustomerRequest
{
    [MaxLength(50)]
    public string? CustomerCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string? CustomerName { get; set; }

    public string? CustomerAddress { get; set; }

    [Required]
    public int CreatedBy { get; set; }
}
