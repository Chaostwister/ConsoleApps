using System;
using System.IO;
using ConsoleApps;
using vocabTrainer;
using saving;

namespace vocabTrainer
{
    [Serializable]
    public struct VocabBookData
    {
        public string name;
        public string fileName;

        public List<string> lang1;
        public List<string> lang2;

        public VocabBookData(string name)
        {
            this.name = name;

            fileName = name + ".dat";

            lang1 = new List<string>();
            lang2 = new List<string>();
        }
    }

    public delegate void ChoiceFunction();

    public struct VocabTrainerChoice
    {
        public readonly ChoiceFunction Function;
        public readonly string Name;

        public VocabTrainerChoice(ChoiceFunction function, string name)
        {
            Function = function;
            Name = name;
        }
    }

    public class VocabTrainer : ConsoleApp
    {
        private static List<VocabBookData> books = new();

        private readonly VocabTrainerChoice[] _choices =
        {
            new(AddBook, "Add Book"),
            new(OpenBook, "Open Book"),
            new(EditBook, "Edit Book"),
            new(LearnBook, "Learn Book")
        };

        public override void Start()
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Vocabs");
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\Vocabs");

            try
            {
                var files = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles();

                foreach (var file in files)
                {
                    books.Add(Saving.LoadData(file.FullName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public override bool Update()
        {
            Console.Clear();

            Console.WriteLine("Vocab Trainer:");

            for (var i = 0; i < _choices.Length; i++)
            {
                Console.WriteLine($" -{_choices[i].Name}{new string(' ', 20 - _choices[i].Name.Length)}[{i}]");
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
                if (option > _choices.Length - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    _choices[option].Function();
                }
            }

            return true;
        }

        private static bool ChooseBook(out VocabBookData data)
        {
            Console.Clear();
            Console.WriteLine("Choose Book:");
            for (var i = 0; i < books.Count; i++)
            {
                Console.WriteLine($" -{books[i].name}{new string(' ', 20 - books[i].name.Length)}[{i}]");
            }

            Console.WriteLine($"\n");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out var option))
            {
                Console.WriteLine("Not a valid option");
            }
            else
            {
                if (option > books.Count - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    data = books[option];
                    return true;
                }
            }

            data = new VocabBookData();
            return false;
        }

        private static void AddBook()
        {
            Console.Clear();
            
            Console.WriteLine("Add book:\nEnter book name:");
            var bookName = Console.ReadLine();
            if(bookName == "") return;
            
            var data = new VocabBookData(bookName);
            Saving.SaveData(data, data.fileName);
            books.Add(data);
        }

        private static void OpenBook()
        {
            if (!ChooseBook(out var data)) return;
            
            Console.Clear();
            Console.WriteLine($"{data.name}:\n");

            for (var i = 0; i < data.lang1.Count; i++)
            {
                Console.WriteLine($"{data.lang1[i]}{new string(' ',20 - data.lang1[i].Length)}{data.lang2[i]}\n");
            }
                
                
            Console.ReadLine();


        }

        private static void EditBook()
        {
            if (!ChooseBook(out var data)) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{data.name}:\n");

                for (var i = 0; i < data.lang1.Count; i++)
                {
                    Console.WriteLine($"{data.lang1[i]}{new string(' ', 20 - data.lang1[i].Length)}{data.lang2[i]}\n");
                }
                
                
                Console.WriteLine("\nEnglish Word:\n");
                var english = Console.ReadLine();
                if(english == "") break;
                Console.WriteLine("\nGerman Word:\n");
                var german = Console.ReadLine();
                if(german == "") break;
                
                data.lang1.Add(english);
                data.lang2.Add(german);
            }
            
            Saving.SaveData(data, data.fileName);
        }

        private static void LearnBook()
        {
            Console.ReadLine();
            Console.WriteLine("learn book");
        }
    }
}


namespace saving
{
    public static class Saving
    {
        // Save data to file
        public static void SaveData(VocabBookData vocabBookData, string fileName)
        {
            try
            {
                using var writer = new BinaryWriter(File.Open(fileName, FileMode.Create));
                // Write the lengths of the arrays
                writer.Write(vocabBookData.lang1.Count);
                writer.Write(vocabBookData.lang2.Count);

                // Write each string in the arrays
                foreach (var item in vocabBookData.lang1)
                    writer.Write(item);

                foreach (var item in vocabBookData.lang2)
                    writer.Write(item);
                writer.Write(vocabBookData.name);
                writer.Write(vocabBookData.fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        // Load data from file
        public static VocabBookData LoadData(string fileName)
        {
            var data = new VocabBookData();
            try
            {
                using var reader = new BinaryReader(File.Open(fileName, FileMode.Open));
                // Read the lengths of the arrays
                var array1Length = reader.ReadInt32();
                var array2Length = reader.ReadInt32();

                // Read each string in the arrays
                data.lang1 = new List<string>();
                for (var i = 0; i < array1Length; i++)
                    data.lang1.Add(reader.ReadString());

                data.lang2 = new List<string>();
                for (var i = 0; i < array2Length; i++)
                    data.lang2.Add(reader.ReadString());

                data.name = reader.ReadString();
                data.fileName = reader.ReadString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
            }

            return data;
        }
    }
}