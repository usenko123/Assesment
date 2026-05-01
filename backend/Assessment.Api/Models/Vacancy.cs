using System.ComponentModel.DataAnnotations;

namespace Assessment.Api.Models;

public class Vacancy
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int CompanyId { get; set; }

    public Company? Company { get; set; }
}
