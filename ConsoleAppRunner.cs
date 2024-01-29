namespace ConsoleApps;

public static class ConsoleAppRunner
{
    
    public struct ConsoleAppData
    {
        public readonly ConsoleApp App;
        public readonly string Name;

        public ConsoleAppData(ConsoleApp app, string name)
        {
            App = app;
            Name = name;
        }
    }

    public static bool Update(ConsoleAppData[] data)
    {
        Console.Clear();
        Console.WriteLine("Choose Option:");
        for (var i = 0; i < data.Length; i++)
        {
            Console.WriteLine($" -{data[i].Name}{new string(' ', 20 - data[i].Name.Length)}[{i}]");
        }
        Console.WriteLine($" -quit{new string(' ', 16)}[q]\n");

        var input = Console.ReadLine();

        if (input == "q") return false;

        if (!int.TryParse(input, out var option))
        {
            Console.WriteLine("Not a valid option");
        }
        else
        {
            if (option > data.Length - 1)
            {
                Console.WriteLine("Not a valid option");
            }
            else
            {
                data[option].App.Start();
                
                var run = true;
                while (run)
                {
                    run = data[option].App.Update();
                }
            }
        }

        return true;
    }
}

public abstract class ConsoleApp
{
    public abstract void Start();
    public abstract bool Update();
}