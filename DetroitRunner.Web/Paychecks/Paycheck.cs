namespace DetroitRunner.Web.Paychecks;

public record Deduction(string Name, decimal Amount);

public record Paycheck(
    DateOnly PayDate,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal GrossPay,
    IReadOnlyList<Deduction> Deductions)
{
    public decimal TotalDeductions => Deductions.Sum(d => d.Amount);

    public decimal NetPay => GrossPay - TotalDeductions;
}
