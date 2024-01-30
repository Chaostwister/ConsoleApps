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

    public delegate void ChoiceFunction(VocabBookData bookData);

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
        private static VocabBookData data;
        private static int dataIdx;

        private readonly VocabTrainerChoice[] _1choices =
        {
            new(AddBook, "Add Book"),
            new(OpenBook, "Open Book"),
            new(DeleteBook, "Delete Book")
        };

        private static readonly VocabTrainerChoice[] _2choices =
        {
            new(EditBook, "Edit Book"),
            new(LearnBook, "Learn Book"),
            new(RenameBook, "Rename book")
            //TODO add renaming
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

            for (var i = 0; i < _1choices.Length; i++)
            {
                Console.WriteLine($" -{_1choices[i].Name}{new string(' ', 20 - _1choices[i].Name.Length)}[{i}]");
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
                if (option > _1choices.Length - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    _1choices[option].Function(data);
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

        private static bool ChooseBook(out VocabBookData chosenData)
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
                    chosenData = _books[option];
                    return true;
                }
            }

            chosenData = new VocabBookData();
            return false;
        }

        private static void AddBook(VocabBookData bookData)
        {
            //TODO check for existing book

            Console.Clear();

            Console.WriteLine("Add book:\nEnter book name:");
            var bookName = Console.ReadLine();
            if (bookName == "") return;

            var newData = new VocabBookData(bookName);
            Saving.SaveData(newData, newData.fileName);
            _books.Add(newData);
        }

        private static void OpenBook(VocabBookData bookData)
        {
            //Todo show vocab list according to window size
            if (!ChooseBook(out data)) return;

            Console.Clear();
            Console.WriteLine($"{data.name}:\n");

            for (var i = 0; i < data.lang1.Count; i++)
            {
                Console.WriteLine($"{data.lang1[i]}{new string(' ', 20 - data.lang1[i].Length)}{data.lang2[i]}\n");
            }

            //Todo fix lenght
            Console.WriteLine($"{new string('-', Console.BufferWidth)}\n");

            for (var i = 0; i < _2choices.Length; i++)
            {
                Console.WriteLine($" -{_2choices[i].Name}{new string(' ', 20 - _2choices[i].Name.Length)}[{i}]");
            }

            var input = Console.ReadLine();

            if (!int.TryParse(input, out var option))
            {
                Console.WriteLine("Not a valid option");
            }
            else
            {
                if (option > _2choices.Length - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    _2choices[option].Function(bookData);
                }
            }

            Console.ReadLine();
        }

        private static void EditBook(VocabBookData bookData)
        {
            VocabTrainerChoice[] editChoices =
            {
                new(AddWord, "Add Word"),
                new(EditWord, "Edit Word")
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Edit book:\n");

                for (var i = 0; i < editChoices.Length; i++)
                {
                    Console.WriteLine(
                        $" -{editChoices[i].Name}{new string(' ', 20 - editChoices[i].Name.Length)}[{i}]");
                }

                var input = Console.ReadLine();
                if (input == "") return;

                if (int.TryParse(input, out var option))
                {
                    editChoices[option].Function(bookData);
                }

                Saving.SaveData(bookData, bookData.fileName);
            }
        }

        private static void AddWord(VocabBookData bookData)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{bookData.name}:\n");

                for (var i = 0; i < bookData.lang1.Count; i++)
                {
                    Console.WriteLine(
                        $"{bookData.lang1[i]}{new string(' ', 20 - bookData.lang1[i].Length)}{bookData.lang2[i]}\n");
                }


                Console.WriteLine("\nEnglish Word:\n");
                var english = Console.ReadLine();
                if (english == string.Empty) break;
                Console.WriteLine("\nGerman Word:\n");
                var german = Console.ReadLine();
                if (german == string.Empty) break;

                if (english != null) bookData.lang1.Add(english);
                if (german != null) bookData.lang2.Add(german);
            }
        }

        private static void EditWord(VocabBookData bookData)
        {
            Console.Clear();

            Console.WriteLine($"{bookData.name}:\nEnter index or word to edit:\n");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var index))
            {
                Console.Clear();
                Console.WriteLine(
                    $"\n{bookData.lang1[index - 1]}{new string(' ', 20 - bookData.lang1[index - 1].Length)}{bookData.lang2[index - 1]}\n");
            }

            Console.WriteLine($"\nEnter new english word, press \"Enter\" to skip\n");

            var newLang1Word = Console.ReadLine();
            if (newLang1Word != "") bookData.lang1[index - 1] = newLang1Word;

            Console.WriteLine($"\nEnter new german word, press \"Enter\" to skip\n");

            var newLang2Word = Console.ReadLine();
            if (newLang2Word != "") bookData.lang2[index - 1] = newLang2Word;
        }

        private static void LearnBook(VocabBookData bookData)
        {
            var vocabAmount = bookData.lang1.Count;
            var random = new Random();

            var pool = new List<int>();


            for (var i = 0; i <= vocabAmount - 1; i++)
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
                Console.WriteLine(bookData.lang2[num] + "\n");

                var input = Console.ReadLine();
                if (input == string.Empty) return;

                if (input == bookData.lang1[num]) Console.WriteLine("\nCorrect\n");
            }

            Console.ReadLine();
        }


        private static void DeleteBook(VocabBookData bookData)
        {
            if (!ChooseBook(out data)) return;
            Console.Clear();
            Console.WriteLine("To confirm deletion type \"delete\"");

            var input = Console.ReadLine();
            if (input != "delete") return;

            File.Delete(data.fileName);

            LoadBooks();
        }

        private static void RenameBook(VocabBookData bookData)
        {
            //TODO check for same/forbidden name
            var input = Console.ReadLine();
            File.Move(Directory.GetCurrentDirectory() + bookData.fileName,
                Directory.GetCurrentDirectory() + input + ".dat");
            bookData.name = input;
            bookData.fileName = input + ".dat";
            Saving.SaveData(bookData, bookData.fileName);
        }
    }
}