using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;

namespace Setl.Cmd;

internal static class SVBWWB65PlusProcessExample
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var pipelineExecutor = new SingleThreadedPipelineExecutor(loggerFactory);
        var logger = loggerFactory.CreateLogger(nameof(pipelineExecutor));
        pipelineExecutor.Starting += _ => logger.LogInformation("Starting");
        pipelineExecutor.Finishing += _ => logger.LogInformation("Finishing");
        var process = new Verwerk65Plus(pipelineExecutor, loggerFactory);
        process.Execute();
    }

    private class Verwerk65Plus : EtlProcess
    {
        private readonly ILoggerFactory loggerFactory;
        
        public Verwerk65Plus(
            IPipelineExecutor pipelineExecutor,
            ILoggerFactory loggerFactory)
            : base(pipelineExecutor, loggerFactory.CreateLogger<Verwerk65Plus>())
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void Initialize()
        {
            const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
            this.Register(new ExtractRows(path, this.loggerFactory));
            this.Register(new WriteRows(this.loggerFactory));
        }
    }

    private class WriteRows : AbstractOperation
    {
        public WriteRows(ILoggerFactory loggerFactory) 
            : base(loggerFactory.CreateLogger<WriteRows>())
        {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                var node = row.ToJsonNode();
                var code = node!["Recordcode"];
                Console.WriteLine($"=> {code}");
                yield return row.Clone();
            }
        }
    }
    
    private class ExtractRows : AbstractOperation
    {
        private readonly string path;

        public ExtractRows(string path, ILoggerFactory loggerFactory) 
            : base(loggerFactory.CreateLogger<ExtractRows>())
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