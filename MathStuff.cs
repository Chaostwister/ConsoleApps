using System.Diagnostics;

namespace School;

public class MathStuff : ConsoleApp
{
    private int _min = -100;
    private int _max = 100;
    private readonly Random _random = new();
    private readonly List<TimeSpan> _times = new();

    public override void Start()
    {
        
    }
    
    public override bool Update()
    {
        Console.Clear();
        Console.WriteLine("Math:\n -start [0]\n -settings [1]\n -quit[q]\n");
        if (_times.Count != 0)
        {
            var avg = new TimeSpan();
            avg = _times.Aggregate(avg, (current, t) => current + t / _times.Count);

            Console.WriteLine($"Average time:{avg}");
        }

        var input = Console.ReadLine();
        if (input == "q") return false;

        if (!int.TryParse(input, out var option)) return true;
        switch (option)
        {
            default:
                Console.Clear();
                Console.WriteLine("not a valid option");
                break;
            case 0:
                Console.Clear();
                StartQuiz();
                break;
            case 1:
                Console.Clear();
                Settings();
                break;
        }

        return true;
    }

    private void StartQuiz()
    {
        Console.WriteLine("Type q to quit\n");
        var num1 = _random.Next(_min, _max);
        var num2 = _random.Next(_min, _max);
        var solved = false;
        Stopwatch watch = new();

        while (true)
        {
            if (solved) UpdateNumbers(out num1, out num2);


            Console.WriteLine($"{num1} + {num2} =");

            if (solved) watch.Restart();
            else watch.Start();
            var input = Console.ReadLine();
            watch.Stop();

            if (input is "q" or "") return;

            if (!int.TryParse(input, out var result)) continue;
            Console.WriteLine((num1 + num2 == result) + "\n" + (num1 + num2 == result ? watch.Elapsed : "") + "\n");
            solved = num1 + num2 == result;
            if (solved) _times.Add(watch.Elapsed);
        }
    }

    private void UpdateNumbers(out int num1, out int num2)
    {
        num1 = _random.Next(_min, _max);
        num2 = _random.Next(_min, _max);
    }

    private void Settings()
    {
        while (true)
        {
            Console.WriteLine(
                $"Settings\n -minimum value: {_min} [0]\n -maximum value: {_max} [1]\n\nPress enter to exit settings");
            var input = Console.ReadLine();

            if (input == string.Empty) return;

            if (!int.TryParse(input, out var option)) return;
            string? value;
            int val;
            switch (option)
            {
                default:
                    Console.Clear();
                    Console.WriteLine("Not a valid option\n");
                    break;
                case 0:
                    Console.Clear();
                    Console.WriteLine("Enter new minimum value\n\nPress Enter to exit");

                    value = Console.ReadLine();

                    Console.Clear();

                    if (int.TryParse(value, out val))
                        _min = val;
                    else
                        Console.WriteLine("Not a valid value");

                    break;
                case 1:
                    Console.Clear();
                    Console.WriteLine("Enter new maximum value\n\nPress Enter to exit");

                    value = Console.ReadLine();

                    Console.Clear();

                    if (int.TryParse(value, out val))
                        _max = val;
                    else
                        Console.WriteLine("Not a valid value");

                    break;
            }
        }
    }
}