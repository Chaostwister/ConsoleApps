namespace School;

internal abstract class Program
{
    private static readonly ConsoleAppRunner.ConsoleAppData[] Data =
    {
        new(new MathStuff(), "Math"), new(new VocabTrainer(), "Vocab trainer")
    };

    private static void Main()
    {
        Console.Clear();

        var run = true;
        while (run)
        {
            run = ConsoleAppRunner.Update(Data);
        }
    }
}