using VacationManagementApi.Enums;
using VacationManagementApi.Models;

namespace VacationManagementApi.Data;

public static class DatabaseInitializer
{
    public static void Initialize(VacationDbContext db)
    {
        // Add administrators
        db.Administrators.AddRange(
            new Administrator { Id = 1, Name = "Alice" },
            new Administrator { Id = 2, Name = "Bob" }
        );

        // Add companies
        db.Companies.AddRange(
            new Company
            {
                Id = 1,
                Name = "Acme Corp",
                CountryCode = "SV",
                CompanyVacationPolicy = new CompanyVacationPolicy
                {
                    Id = 1,
                    CompanyId = 1,
                    MinimumVacationDaysOverride = 0,
                    WeekendCountsAsVacation = false
                }
            },
            new Company
            {
                Id = 2,
                Name = "Globex Inc",
                CountryCode = "NO",
                CompanyVacationPolicy = new CompanyVacationPolicy
                {
                    Id = 2,
                    CompanyId = 2,
                    MinimumVacationDaysOverride = -1,
                    WeekendCountsAsVacation = false
                }
            }
        );

        // Add employees
        db.Employees.AddRange(
            new Employee
            {
                Id = 1,
                Name = "John Doe",
                Department = "Design",
                Role = "Design Lead",
                Status = EmployeeStatus.Active,
                VacationDaysOverride = 0,
                CompanyId = 1,
                Email = "john.doe@example.com"
            },
            new Employee
            {
                Id = 2,
                Name = "Jane Doe",
                Department = "IT",
                Role = "Senior Software Engineer",
                Status = EmployeeStatus.Active,
                VacationDaysOverride = 0,
                CompanyId = 1,
                Email = "jane.doe@example.com"
            },
            new Employee
            {
                Id = 3,
                Name = "John Smith",
                Department = "Management",
                Role = "CFO",
                Status = EmployeeStatus.Retired,
                VacationDaysOverride = 5,
                CompanyId = 2,
                Email = "john.smith@example.com"
            }
        );

        db.SaveChanges();
    }
}
