using Setl;

internal static class SVBWWB65PlusProcessExample
{
    public static void Run()
    {
        const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
        var op = new ExtractRows(path);
        var rows = op.Execute([]);
        foreach (dynamic row in rows)
        {
            Console.WriteLine(row.Recordcode);
        }
    }

    private class ExtractRows : AbstractOperation
    {
        private readonly string path;

        public ExtractRows(string path)
        {
            this.path = path;
        }
    
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using var stream = File.OpenRead(this.path);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
    
                yield return WWB65Plus.Parser.Parse(line);
            }
        }
    }
}