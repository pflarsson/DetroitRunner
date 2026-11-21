using DetroitRunner;
using Lamar;

class Program
{
    static async Task Main(string[] args)
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
              //  scanner.AssemblyContainingType<RabbitListener>();
              scanner.AssemblyContainingType<SimpleStartable>();
                scanner.AddAllTypesOf<IStartable>();
            });
        });

        var startables = container.GetAllInstances<IStartable>();

        foreach (var startable in startables)
        {
            await startable.StartAsync();
        }

        Console.WriteLine("Worker running...");
        await Task.Delay(-1);
    }
}