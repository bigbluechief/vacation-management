using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VacationManagementApi.Enums;

namespace VacationManagementApi.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }
    required public string Name { get; set; }
    required public string Department { get; set; }
    required public string Role { get; set; }
    required public EmployeeStatus Status { get; set; }
    required public int VacationDaysOverride { get; set; }
    [ForeignKey(nameof(Company))]
    required public int CompanyId { get; set; }
    required public string Email { get; set; }

    public Company Company { get; set; } = default!;

    public ICollection<VacationBalance> VacationBalances { get; set; } = [];
    public ICollection<VacationRequest> VacationRequests { get; set; } = [];
}