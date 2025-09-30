namespace Setl.Cmd.Examples.V2;

public class AowAioExample
{
    public static void Run()
    {
        const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
        foreach (var row in Extract(path))
        {
            Console.WriteLine(row["Recordcode"]);   
        }
    }

    private static IEnumerable<Row> Extract(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (AowAio.Parser.TryParse(line, out var row))
            {
                yield return row;
            }
        }
    }
}