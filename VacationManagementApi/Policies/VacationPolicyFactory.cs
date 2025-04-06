using VacationManagementApi.Models;

namespace VacationManagementApi.Policies;

public static class VacationPolicyFactory
{

    public static IVacationPolicy For(Company company)
    {
        if (string.IsNullOrWhiteSpace(company.CountryCode))
            throw new ArgumentException("Company is missing CountryCode");

        return company.CountryCode switch
        {
            "NO" => new NorwegianVacationPolicy(),
            "SV" => new SwedenVacationPolicy(),
            "US" => new UsaVacationPolicy(),
            _ => throw new NotImplementedException(),
        };
    }

    public static IVacationPolicy For(Employee employee)
    {
        return For(employee.Company);
    }

}
