using System.ComponentModel.DataAnnotations;

namespace Assessment.Api.Models;

public class Company
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
