namespace DetroitRunner.Web.Paychecks;

// Demo data: one monthly paycheck, paid on the last day of the month.
public class SamplePaycheckSource : IPaycheckSource
{
    private const decimal MonthlyGross = 5000m;

    public Paycheck? GetForMonth(int year, int month)
    {
        var periodStart = new DateOnly(year, month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        Deduction[] deductions =
        [
            new("Federal income tax", 750.00m),
            new("Michigan income tax (4.25%)", MonthlyGross * 0.0425m),
            new("Social Security (6.2%)", MonthlyGross * 0.062m),
            new("Medicare (1.45%)", MonthlyGross * 0.0145m),
            new("401(k) (5%)", MonthlyGross * 0.05m),
        ];

        return new Paycheck(periodEnd, periodStart, periodEnd, MonthlyGross, deductions);
    }
}
