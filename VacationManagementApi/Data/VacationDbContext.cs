using Microsoft.EntityFrameworkCore;
using VacationManagementApi.Enums;
using VacationManagementApi.Models;

namespace VacationManagementApi.Data;

public class VacationDbContext(DbContextOptions<VacationDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyVacationPolicy> CompanyVacationPolicies => Set<CompanyVacationPolicy>();
    public DbSet<VacationRequest> VacationRequests => Set<VacationRequest>();
    public DbSet<VacationBalance> VacationBalances => Set<VacationBalance>();
}
