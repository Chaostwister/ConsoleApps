namespace School;

public static class Saving
{
    public static void SaveData(VocabBookData vocabBookData, string fileName)
    {
        try
        {
            using var writer = new BinaryWriter(File.Open(fileName, FileMode.Create));
            writer.Write(vocabBookData.Lang1.Count);
            writer.Write(vocabBookData.Lang2.Count);

            foreach (var item in vocabBookData.Lang1) writer.Write(item);

            foreach (var item in vocabBookData.Lang2) writer.Write(item);


            writer.Write(vocabBookData.Name);
            writer.Write(vocabBookData.FileName);

            foreach (var item in vocabBookData.Repetitions) writer.Write(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }

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
            data.Lang1 = new List<string>();
            for (var i = 0; i < array1Length; i++)
                data.Lang1.Add(reader.ReadString());

            data.Lang2 = new List<string>();
            for (var i = 0; i < array2Length; i++)
                data.Lang2.Add(reader.ReadString());

            data.Name = reader.ReadString();
            data.FileName = reader.ReadString();


            data.Repetitions = new List<int>();
            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                for (var i = 0; i < array2Length; i++)
                    data.Repetitions.Add(reader.ReadInt32());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
        }

        return data;
    }
}