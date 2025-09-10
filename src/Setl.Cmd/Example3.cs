using Microsoft.Extensions.Logging;
using Setl;

internal class Example3
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<Example3>();
        var executor = new SingleThreadedPipelineExecutor(logger);
        using var process = new TestProcess(logger, executor);
        process.Execute();
    }

    private class TestProcess : EtlProcess
    {
        private readonly ILogger logger;
        
        public TestProcess(
            ILogger logger, 
            IPipelineExecutor pipelineExecutor) 
            : base(logger, pipelineExecutor)
        {
            this.logger = logger;       
        }

        protected override void Initialize()
        {
            var join = 
                new TestJoin(this.logger)
                    .Left(new ExtractSubjects(this.logger))
                    .Right(new ExtractOrgs(this.logger));
            this.Register(join);
            this.Register(new WriteRow(this.logger));
        }
    }
    
    private class Subject
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        
        public int OrgId { get; set; }
    }

    private class Org
    {
        public int Id { get; set; }
        
        public string? Code { get; set; }
        
        public string? Name { get; set; }   
    }

    private class WriteRow : AbstractOperation
    {
        public WriteRow(ILogger logger) : base(logger)
        {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                this.LogInformation("Write: {row}", row);
                yield return row;
            }
        }
    }
    
    private class TestJoin : NestedLoopsJoinOperation
    {
        public TestJoin(ILogger logger) : base(logger)
        {
        }

        protected override Row MergeRows(Row leftRow, Row rightRow)
        {
            var row = leftRow.Clone();
            row["OrgCode"] = rightRow["Code"];
            row["OrgName"] = rightRow["Name"];
            return row;
        }

        protected override void LeftOrphanRow(Row row)
        {
            this.LogInformation("Left orphan row: {row}", row);
        }

        protected override bool MatchJoinCondition(Row leftRow, Row rightRow)
        {
            // Inner join
            // return Equals(leftRow["OrgId"], rightRow["Id"]);
            
            // Left join
            // return 
            //     Equals(leftRow["OrgId"], rightRow["Id"]) || 
            //     rightRow["Id"] == null;
            
            // Right join
            return
                object.Equals(leftRow["OrgId"], rightRow["Id"]) || 
                leftRow["OrgId"] == null;
        }
    }

    private class ExtractSubjects : AbstractOperation
    {
        private readonly List<Subject> subjects =
        [
            new()
            {
                Id = 1,
                Name = "Subject_One",
                OrgId = 1,
            },
            new()
            {
                Id = 2,
                Name = "Subject_Two",
                OrgId = 1,
            },
            new()
            {
                Id = 3,
                Name = "Subject_Three",
                OrgId = 2,
            },
            new()
            {
                Id = 4,
                Name = "Subject_Four",
                OrgId = 3,
            },
        ];
        
        public ExtractSubjects(ILogger logger) : base(logger)
        {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
            this.subjects.Select(Row.FromObject);
    }
    
    private class ExtractOrgs : AbstractOperation
    {
        private readonly List<Org> orgs =
        [
            new()
            {
                Id = 1,
                Code = "ABC",
                Name = "Org_One",
            },
            new()
            {
                Id = 2,
                Code = "DEF",
                Name = "Org_Two",
            },
            new()
            {
                Id = 4,
                Code = "GHI",
                Name = "Org_Four",
            }
        ];
        
        public ExtractOrgs(ILogger logger) : base(logger)
        {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
            this.orgs.Select(Row.FromObject);
    }
}