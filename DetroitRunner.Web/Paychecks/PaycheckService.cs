namespace DetroitRunner.Web.Paychecks;

public class PaycheckService(IPaycheckSource source, TimeProvider timeProvider)
{
    public Paycheck? GetLastMonthsPaycheck()
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var lastMonth = today.AddMonths(-1);
        return source.GetForMonth(lastMonth.Year, lastMonth.Month);
    }
}
