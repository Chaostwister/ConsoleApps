namespace School
{
    [Serializable]
    public struct VocabBookData
    {
        public string Name;
        public string FileName;

        public List<string> Lang1;
        public List<string> Lang2;

        public List<int> Repetitions;

        public VocabBookData(string name)
        {
            Name = name;

            FileName = name + ".dat";

            Lang1 = new List<string>();
            Lang2 = new List<string>();

            Repetitions = new List<int>();
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
        private static VocabBookData _data;

        private readonly VocabTrainerChoice[] _choices1 =
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

            if (path != null) Directory.SetCurrentDirectory(path);


            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Vocabs");
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/Vocabs");

            LoadBooks();
        }

        public override bool Update()
        {
            Console.Clear();

            Console.WriteLine("Vocab Trainer:");

            SetupOptions(_choices1);

            Console.WriteLine($" -quit{new string(' ', 16)}[q]\n");

            var input = Console.ReadLine();

            if (input == "q") return false;

            if (input != null) HandleOptions(input, _choices1);

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
                    if (choices[option].Run) choices[option].Function(_data);
                }
            }
        }

        private static void LoadBooks()
        {
            Console.Clear();
            try
            {
                var files = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles();

                _books = new List<VocabBookData>();
                foreach (var file in files)
                {
                    if (file.Name == "settings.dat") continue;
                    _books.Add(Saving.LoadData(file.FullName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            //Console.ReadLine();
        }

        private static bool ChooseBook(out VocabBookData chosenData)
        {
            Console.Clear();
            Console.WriteLine("Choose Book:");
            for (var i = 0; i < _books.Count; i++)
            {
                Console.WriteLine($" -{_books[i].Name}{new string(' ', 20 - _books[i].Name.Length)}[{i}]");
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
            if (bookName != null && !Dependencies.IsValidFileName(bookName)) return;
            if (_books.Any(book => book.Name == bookName))
            {
                return;
            }

            if (bookName == null) return;
            var newData = new VocabBookData(bookName);
            Saving.SaveData(newData, newData.FileName);
            _books.Add(newData);
        }

        private static void OpenBook(VocabBookData bookData)
        {
            //Todo show vocab list according to window size


            if (!ChooseBook(out _data)) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{_data.Name}:\n");

                WriteOutBook(_data.Lang1, _data.Lang2, _data.Repetitions);

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

                Saving.SaveData(bookData, bookData.FileName);
            }
        }

        private static void AddWord(VocabBookData bookData)
        {
            //Todo add editing words
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{bookData.Name}:\n");

                WriteOutBook(_data.Lang1, _data.Lang2);


                Console.WriteLine("\nEnglish Word:\n");
                var english = Console.ReadLine();
                if (english == string.Empty) break;
                Console.WriteLine("\nGerman Word:\n");
                var german = Console.ReadLine();
                if (german == string.Empty) break;

                if (english != null) bookData.Lang1.Add(english);
                if (german != null) bookData.Lang2.Add(german);

                bookData.Repetitions.Add(0);
            }
        }

        private static void EditWord(VocabBookData bookData)
        {
            Console.Clear();

            WriteOutBook(bookData.Lang1, bookData.Lang2);

            Console.WriteLine($"{new string('-', Console.BufferWidth)}\n");


            Console.WriteLine($"{bookData.Name}:\nEnter index or word to edit:\n");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var index))
            {
                Console.Clear();
                Console.WriteLine(
                    $"\n{bookData.Lang1[index - 1]}{new string(' ', 20 - bookData.Lang1[index - 1].Length)}{bookData.Lang2[index - 1]}\n");
            }
            else
            {
                index = (input != null && bookData.Lang1.IndexOf(input) != -1 ? bookData.Lang1.IndexOf(input) :
                    input != null && bookData.Lang2.IndexOf(input) != -1 ? bookData.Lang2.IndexOf(input) : -1) + 1;

                if (index == 0)
                {
                    Console.WriteLine("not found");
                    return;
                }
            }

            Console.WriteLine($"\nEnter new english word, press \"Enter\" to skip\n");

            var newLang1Word = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLang1Word) && !string.IsNullOrEmpty(newLang1Word))
                bookData.Lang1[index - 1] = newLang1Word;

            Console.WriteLine($"\nEnter new german word, press \"Enter\" to skip\n");

            var newLang2Word = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLang2Word) && !string.IsNullOrEmpty(newLang2Word))
                bookData.Lang2[index - 1] = newLang2Word;
        }

        private static void LearnBook(VocabBookData bookData)
        {
            //Todo add words with multiple answers
            //Todo coming back to wrong words/keeping track longer than once

            var wrongWords = new List<int>();

            Console.Clear();
            Console.WriteLine("Quiz:\n");

            foreach (var num in ShufflePool(bookData.Lang1.Count))
            {
                Console.WriteLine(bookData.Lang2[num] + "\n");

                var input = Console.ReadLine();
                if (input is "quit" or "q")
                {
                    Saving.SaveData(bookData, bookData.FileName);
                    return;
                }

                var correct = input == bookData.Lang1[num];
                Console.WriteLine(correct ? "\nCorrect\n" : $"\nWrong: {bookData.Lang1[num]}\n");
                if (!correct)
                {
                    wrongWords.Add(num);
                    if (bookData.Repetitions[num] > 0) bookData.Repetitions[num]--;
                }

                else bookData.Repetitions[num]++;
            }

            if (wrongWords.Count == 0) return;

            Console.WriteLine("Continue with incorrect words:");
            Console.ReadLine();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wrong words:\n");

                var words = wrongWords;
                wrongWords = new List<int>();

                foreach (var num in ShufflePool(words.Count))
                {
                    Console.WriteLine(bookData.Lang2[words[num]] + "\n");

                    var input = Console.ReadLine();
                    if (input is "quit" or "q")
                    {
                        Saving.SaveData(bookData, bookData.FileName);
                        return;
                    }

                    var correct = input == bookData.Lang1[words[num]];
                    Console.WriteLine(correct ? "\nCorrect\n" : $"\nWrong: {bookData.Lang1[words[num]]}\n");
                    if (!correct) wrongWords.Add(words[num]);
                }

                if (wrongWords.Count == 0) break;
                Console.ReadLine();
            }

            Saving.SaveData(bookData, bookData.FileName);
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
            // ReSharper disable once CommentTypo
            // var bookdata is null/empty

            if (!ChooseBook(out _data)) return;
            Console.Clear();
            Console.WriteLine($"To confirm deletion of {_data.Name}, type \"delete\"");

            var input = Console.ReadLine();
            if (input != "delete") return;

            File.Delete(_data.FileName);

            LoadBooks();
        }

        private static void RenameBook(VocabBookData bookData)
        {
            Console.Clear();

            Console.WriteLine($"Rename {bookData.Name} to:\n");

            var input = Console.ReadLine();

            if (input != null && !Dependencies.IsValidFileName(input)) return;

            if (_books.Any(book => book.Name == input))
            {
                return;
            }

            File.Move($"{Directory.GetCurrentDirectory()}/{bookData.FileName}",
                $"{Directory.GetCurrentDirectory()}/{input}.dat");
            if (input != null)
            {
                bookData.Name = input;
                bookData.FileName = input + ".dat";
            }

            Saving.SaveData(bookData, bookData.FileName);
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

        private static void WriteOutBook(List<string> lang1, List<string> lang2, List<int> repetitions)
        {
            WriteOutBook(lang1, lang2);
            var percentage = repetitions.Sum(t => 100f / repetitions.Count / 3f * (t < 4f ? t : 3f));
            Console.WriteLine($"Learned {percentage}%\n");

            // for (var i = 0; i < repetitions.Count; i++)
            // {
            //     if(repetitions[i]>3) Console.WriteLine($"{lang2[i]} {repetitions[i]}\n");
            // }
            //Console.ReadLine();
        }
    }
}