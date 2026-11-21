using DetroitRunner;

public class SimpleStartable : IStartable
{
    public Task StartAsync()
    {
        // Kick off the loop in the background and return immediately so the
        // remaining startables in Program.Main still get started.
        _ = RunAsync();
        return Task.CompletedTask;
    }

    private static async Task RunAsync()
    {
        var count = 0;
        while (true)
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            Console.WriteLine($"SimpleStartable has started. {count++}");
        }
    }
}
