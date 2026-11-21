using DetroitRunner.Web.Paychecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPaycheckSource, SamplePaycheckSource>();
builder.Services.AddSingleton<PaycheckService>();

var app = builder.Build();

app.MapRazorPages();

app.Run();
