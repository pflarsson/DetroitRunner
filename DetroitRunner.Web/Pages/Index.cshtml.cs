using System.Globalization;
using DetroitRunner.Web.Paychecks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetroitRunner.Web.Pages;

public class IndexModel(PaycheckService paychecks) : PageModel
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public Paycheck? Paycheck { get; private set; }

    public void OnGet()
    {
        Paycheck = paychecks.GetLastMonthsPaycheck();
    }

    public static string Money(decimal amount) => amount.ToString("C", UsCulture);

    public static string Date(DateOnly date) => date.ToString("MMMM d, yyyy", UsCulture);
}
