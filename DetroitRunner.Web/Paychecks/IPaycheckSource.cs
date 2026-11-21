namespace DetroitRunner.Web.Paychecks;

// Where paychecks come from. Swap SamplePaycheckSource for a real payroll
// integration by registering a different implementation in Program.cs.
public interface IPaycheckSource
{
    Paycheck? GetForMonth(int year, int month);
}
