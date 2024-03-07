namespace School;

public static class Dependencies
{
    public static int LongestElement(this List<string> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentException("The list is null or empty.");
        }

        var idx = 0;

        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].Length > list[idx].Length)
            {
                idx = i;
            }
        }

        return idx;
    }

    public static int LongestElement(this List<string> list, out int length)
    {
        var idx = LongestElement(list);
        length = list[idx].Length;
        return idx;
    }
    
    public static bool IsValidFileName(string name)
    {
        return !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && !name.Contains(':') && !name.Contains
                   ('?') && !name.Contains
                   ('\\') &&
               !name.Contains('/') && !name.Contains('|') && !name.Contains('*') && !name.Contains('"') &&
               !name.Contains('<') && !name.Contains('>') && name.Length < 20;
    }
}

public delegate void ChoiceFunction();

public struct Choice
{
    public readonly ChoiceFunction Function;
    public readonly string Name;
    public readonly string Id;
    public readonly bool Run;

    public Choice(ChoiceFunction function, string name, string id, bool run)
    {
        Function = function;
        Name = name;
        Id = id;
        Run = run;
    }

    public Choice(ChoiceFunction function, string name, string id)
    {
        Function = function;
        Name = name;
        Id = id;
        Run = true;
    }
}

