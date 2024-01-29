using ConsoleApps;
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
        private static List<VocabBookData> _books = new();

        private readonly VocabTrainerChoice[] _choices =
        {
            new(AddBook, "Add Book"),
            new(OpenBook, "Open Book"),
            new(EditBook, "Edit Book"),
            new(LearnBook, "Learn Book"),
            new(DeleteBook, "Delete Book")
        };

        public override void Start()
        {
            var path = Directory.GetCurrentDirectory();
            while (Path.GetFileName(path) != "ConsoleApps")
            {
                path = Path.GetDirectoryName(path);
            }

            Directory.SetCurrentDirectory(path);


            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Vocabs");
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/Vocabs");

            LoadBooks();
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

        private static void LoadBooks()
        {
            try
            {
                var files = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles();

                _books = new List<VocabBookData>();
                foreach (var file in files)
                {
                    _books.Add(Saving.LoadData(file.FullName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static bool ChooseBook(out VocabBookData data)
        {
            Console.Clear();
            Console.WriteLine("Choose Book:");
            for (var i = 0; i < _books.Count; i++)
            {
                Console.WriteLine($" -{_books[i].name}{new string(' ', 20 - _books[i].name.Length)}[{i}]");
            }

            Console.WriteLine($"\n");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out var option))
            {
                Console.WriteLine("Not a valid option");
            }
            else
            {
                if (option > _books.Count - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    data = _books[option];
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
            if (bookName == "") return;

            var data = new VocabBookData(bookName);
            Saving.SaveData(data, data.fileName);
            _books.Add(data);
        }

        private static void OpenBook()
        {
            if (!ChooseBook(out var data)) return;

            Console.Clear();
            Console.WriteLine($"{data.name}:\n");

            for (var i = 0; i < data.lang1.Count; i++)
            {
                Console.WriteLine($"{data.lang1[i]}{new string(' ', 20 - data.lang1[i].Length)}{data.lang2[i]}\n");
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
                if (english == string.Empty) break;
                Console.WriteLine("\nGerman Word:\n");
                var german = Console.ReadLine();
                if (german == string.Empty) break;

                if (english != null) data.lang1.Add(english);
                if (german != null) data.lang2.Add(german);
            }

            Saving.SaveData(data, data.fileName);
        }

        private static void LearnBook()
        {
            if (!ChooseBook(out var data)) return;

            var vocabAmount = data.lang1.Count;
            var random = new Random();

            var pool = new List<int>();


            for (var i = 0; i <= vocabAmount -1; i++)
            {
                pool.Add(i);
            }

            while (vocabAmount > 1)
            {
                vocabAmount--;
                var k = random.Next(vocabAmount + 1);
                (pool[k], pool[vocabAmount]) = (pool[vocabAmount], pool[k]);
            }
            
            Console.Clear();
            Console.WriteLine("Quiz:\n");

            foreach (var num in pool)
            {
                
                Console.WriteLine(data.lang2[num] + "\n");

                var input = Console.ReadLine();
                if (input == string.Empty) return;

                if (input == data.lang1[num]) Console.WriteLine("\nCorrect\n");
            }

            Console.ReadLine();
        }


        private static void DeleteBook()
        {
            if (!ChooseBook(out var data)) return;
            Console.Clear();
            Console.WriteLine("To confirm deletion type \"delete\"");

            var input = Console.ReadLine();
            if (input != "delete") return;

            File.Delete(data.fileName);

            LoadBooks();
        }
    }
}