using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManagementApi.Models;

public class CompanyVacationPolicy
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    public int CompanyId { get; set; }

    public Company Company { get; set; } = default!;

    public int MinimumVacationDaysOverride { get; set; }
    public bool WeekendCountsAsVacation { get; set; }
}