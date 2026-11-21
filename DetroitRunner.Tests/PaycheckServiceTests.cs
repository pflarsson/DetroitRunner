using DetroitRunner.Web.Paychecks;

namespace DetroitRunner.Tests;

public class PaycheckServiceTests
{
    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private static PaycheckService CreateService(int year, int month, int day) =>
        new(new SamplePaycheckSource(), new FixedTimeProvider(new DateTimeOffset(year, month, day, 12, 0, 0, TimeSpan.Zero)));

    [Theory]
    [InlineData(2026, 9, 24, 2026, 8, 31)]
    [InlineData(2026, 1, 15, 2025, 12, 31)] // rolls back into the previous year
    [InlineData(2026, 3, 31, 2026, 2, 28)]  // shorter previous month
    public void GetLastMonthsPaycheck_ReturnsPaycheckForPreviousCalendarMonth(
        int year, int month, int day, int payYear, int payMonth, int payDay)
    {
        var paycheck = CreateService(year, month, day).GetLastMonthsPaycheck();

        Assert.NotNull(paycheck);
        Assert.Equal(new DateOnly(payYear, payMonth, 1), paycheck.PeriodStart);
        Assert.Equal(new DateOnly(payYear, payMonth, payDay), paycheck.PeriodEnd);
        Assert.Equal(paycheck.PeriodEnd, paycheck.PayDate);
    }

    [Fact]
    public void NetPay_IsGrossPayMinusAllDeductions()
    {
        var paycheck = new Paycheck(
            new DateOnly(2026, 8, 31), new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 31),
            GrossPay: 1000m,
            Deductions: [new("Tax", 150m), new("401(k)", 50m)]);

        Assert.Equal(200m, paycheck.TotalDeductions);
        Assert.Equal(800m, paycheck.NetPay);
    }
}
