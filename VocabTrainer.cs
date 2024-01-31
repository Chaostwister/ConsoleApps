using ConsoleApps;
using saving;
using dependencies;

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
        public readonly bool Run;

        public VocabTrainerChoice(ChoiceFunction function, string name, bool run)
        {
            Function = function;
            Name = name;
            Run = run;
        }

        public VocabTrainerChoice(ChoiceFunction function, string name)
        {
            Function = function;
            Name = name;
            Run = true;
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

            SetupOptions(_1choices);

            Console.WriteLine($" -quit{new string(' ', 16)}[q]\n");

            var input = Console.ReadLine();

            if (input == "q") return false;

            HandleOptions(input, _1choices);

            return true;
        }

        private static void SetupOptions(IList<VocabTrainerChoice> choices)
        {
            for (var i = 0; i < choices.Count; i++)
            {
                Console.WriteLine(choices[i].Run
                    ? $" -{choices[i].Name}{new string(' ', 20 - choices[i].Name.Length)}[{i}]"
                    : $" -{choices[i].Name}{new string(' ', 20 - choices[i].Name.Length)}[-]");
            }
        }

        private static void HandleOptions(string input, VocabTrainerChoice[] choices)
        {
            if (!int.TryParse(input, out var option))
            {
                Console.WriteLine("Not a valid option");
            }
            else
            {
                if (option > choices.Length - 1)
                {
                    Console.WriteLine("Not a valid option");
                }
                else
                {
                    if (choices[option].Run) choices[option].Function(data);
                }
            }
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
            Console.Clear();

            Console.WriteLine("Add book:\nEnter book name:");
            var bookName = Console.ReadLine();
            if (bookName == "" || string.IsNullOrWhiteSpace(bookName)) return;
            if (_books.Any(book => book.name == bookName))
            {
                return;
            }

            var newData = new VocabBookData(bookName);
            Saving.SaveData(newData, newData.fileName);
            _books.Add(newData);
        }

        private static void OpenBook(VocabBookData bookData)
        {
            //Todo show vocab list according to window size


            if (!ChooseBook(out data)) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{data.name}:\n");

                WriteOutBook(data.lang1, data.lang2);

                Console.WriteLine($"{new string('-', Console.BufferWidth)}\n");

                SetupOptions(_2choices);

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) break;

                HandleOptions(input, _2choices);
            }
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
                        $" -{editChoices[i].Name}" +
                        $"{new string(' ', 20 - editChoices[i].Name.Length)}" +
                        $"[{i}]");
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
            //Todo add editing words
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{bookData.name}:\n");

                WriteOutBook(data.lang1, data.lang2);


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
            
            WriteOutBook(bookData.lang1, bookData.lang2);
            
            Console.WriteLine($"{new string('-', Console.BufferWidth)}\n");
            

            Console.WriteLine($"{bookData.name}:\nEnter index or word to edit:\n");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var index))
            {
                Console.Clear();
                Console.WriteLine(
                    $"\n{bookData.lang1[index - 1]}{new string(' ', 20 - bookData.lang1[index - 1].Length)}{bookData.lang2[index - 1]}\n");
            }
            else
            {
                index = (bookData.lang1.IndexOf(input) != -1 ? bookData.lang1.IndexOf(input) :
                    bookData.lang2.IndexOf(input) != -1 ? bookData.lang2.IndexOf(input) : -1) + 1;

                if (index == 0)
                {
                    Console.WriteLine("not found");
                    return;
                }
            }

            Console.WriteLine($"\nEnter new english word, press \"Enter\" to skip\n");

            var newLang1Word = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLang1Word) && !string.IsNullOrEmpty(newLang1Word))
                bookData.lang1[index - 1] = newLang1Word;

            Console.WriteLine($"\nEnter new german word, press \"Enter\" to skip\n");

            var newLang2Word = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLang2Word) && !string.IsNullOrEmpty(newLang2Word))
                bookData.lang2[index - 1] = newLang2Word;
        }

        private static void LearnBook(VocabBookData bookData)
        {
            //Todo add words with multiple answers
            //Todo coming back to wrong words/keeping track longer than once

            var wrongWords = new List<int>();

            Console.Clear();
            Console.WriteLine("Quiz:\n");

            foreach (var num in ShufflePool(bookData.lang1.Count))
            {
                Console.WriteLine(bookData.lang2[num] + "\n");

                var input = Console.ReadLine();
                if (input == string.Empty) return;

                var correct = input == bookData.lang1[num];
                Console.WriteLine(correct ? "\nCorrect\n" : $"\nWrong: {bookData.lang1[num]}\n");
                if (!correct) wrongWords.Add(num);
            }

            Console.WriteLine("Continue with incorrect words:");
            Console.ReadLine();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wrong words:\n");

                var words = wrongWords;
                wrongWords = new List<int>();

                foreach (var num in ShufflePool(bookData.lang1.Count))
                {
                    Console.WriteLine(bookData.lang2[num] + "\n");

                    var input = Console.ReadLine();
                    if (input == string.Empty) return;

                    var correct = input == bookData.lang1[num];
                    Console.WriteLine(correct ? "\nCorrect\n" : $"\nWrong: {bookData.lang1[num]}\n");
                    if (!correct) wrongWords.Add(num);
                }

                if (wrongWords.Count == 0) break;
            }

            Console.ReadLine();
        }

        private static List<int> ShufflePool(int size)
        {
            var random = new Random();
            var pool = new List<int>();

            for (var i = 0; i <= size - 1; i++)
            {
                pool.Add(i);
            }

            while (size > 1)
            {
                size--;
                var k = random.Next(size + 1);
                (pool[k], pool[size]) = (pool[size], pool[k]);
            }

            return pool;
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
            Console.Clear();

            Console.WriteLine($"Rename {bookData.name} to:\n");

            var input = Console.ReadLine();

            if (input == "" || string.IsNullOrWhiteSpace(input)) return;

            if (_books.Any(book => book.name == input))
            {
                return;
            }

            File.Move($"{Directory.GetCurrentDirectory()}/{bookData.fileName}",
                $"{Directory.GetCurrentDirectory()}/{input}.dat");
            bookData.name = input;
            bookData.fileName = input + ".dat";
            Saving.SaveData(bookData, bookData.fileName);
            LoadBooks();
        }

        private static void WriteOutBook(List<string> lang1, List<string> lang2)
        {
            for (var i = 0; i < lang1.Count; i++)
            {
                var pad =
                    20 - lang1[lang1.LongestElement()].Length < 10
                        ? lang1[lang1.LongestElement()].Length + 10 - lang1[i].Length
                        : 20 - lang1[i].Length;

                Console.WriteLine(
                    $"{lang1[i]}" +
                    $"{new string(' ', pad)}" +
                    $"{lang2[i]}\n");
            }
        }
    }
}