using VacationManagementApi.Data;
using Microsoft.EntityFrameworkCore;
using VacationManagementApi.Models;

namespace VacationManagementApi.Services;

public class CompanyService(VacationDbContext db)
{
    private readonly VacationDbContext _db = db;

    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        var company = await _db.Companies
                    .FirstOrDefaultAsync(c => c.Id == id);

        return company;
    }
}
