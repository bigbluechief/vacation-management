using System.ComponentModel.DataAnnotations;

namespace VacationManagementApi.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    required public string Name { get; set; }

    required public string CountryCode { get; set; } = default!;

    public CompanyVacationPolicy? CompanyVacationPolicy { get; set; }
}

