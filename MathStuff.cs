namespace School;

public class MathStuff : ConsoleApp
{
    private readonly Choice[] _choices =
    {
        new Choice(Settings, "Settings", "0"),
        new Choice(Quit, "Quit", "q")
    };

    public override void Start()
    {
        
    }

    public override bool Update()
    {
        Clear();
        SetupOptions(_choices);
        Read();
        
        return true;
    }

    private static void SetupOptions(IEnumerable<Choice> choices)
    {
        foreach (var choice in choices)
        {
            Console.WriteLine(choice.Run
                ? $" -{choice.Name}{new string(' ', 20 - choice.Name.Length)}[{choice.Id}]"
                : $" -{choice.Name}{new string(' ', 20 - choice.Name.Length)}[-]");
        }
    }

    private static void Settings()
    {
    }

    private static void Quit()
    {
        
    }

    private static void Clear()
    {
        Console.Clear();
    }

    private static string? Read()
    {
        return Console.ReadLine();
    }
}