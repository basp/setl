using Microsoft.Extensions.Logging;
using Moq;
using Setl.Operations;

namespace Setl.Tests;

public class NestedLoopsJoinOperationTests
{
    public class TestJoinOp : NestedLoopsJoinOperation
    {
        private readonly Func<Row, Row, bool> match;

        public TestJoinOp(Func<Row, Row, bool> match, ILogger logger) 
            : base(logger)
        {
            this.match = match;
        }

        protected override Row MergeRows(Row leftRow, Row rightRow)
        {
            var merged = leftRow.Clone();
            merged["bar"] = rightRow["bar"];
            return merged;
        }

        protected override bool MatchJoinCondition(Row leftRow, Row rightRow) =>
            this.match(leftRow, rightRow);
    }

    public class TestExtract : AbstractOperation
    {
        private readonly object?[] objects;
        
        public TestExtract(ILogger logger, params object?[] objects)
            : base(logger)
        {
            this.objects = objects;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
            this.objects.Select(Row.FromObject);
    }
    
    [Fact]
    public void Sandbox()
    {
        var logger = new Mock<ILogger<TestJoinOp>>();
        var loggerFactory = new Mock<ILoggerFactory>();
        
        var obj1 = new { id = 123, bar_id = 456, foo = "bar" };
        var obj2 = new { id = 456, bar = "quux" };

        var op = new TestJoinOp(Match, logger.Object);
        op.Left(new TestExtract(logger.Object, obj1)); 
        op.Right(new TestExtract(logger.Object, obj2));
        
        var pipelineExecutor = 
            new SingleThreadedPipelineExecutor(loggerFactory.Object);

        // Usually the pipeline executor is set by the ETL process hosting
        // the operation, but since we have no process here, we need to prepare
        // the operation manually.
        op.Prepare(pipelineExecutor);
        
        var result = op.Execute([]).ToList();
        
        Assert.Single(result);
        Assert.Equal("quux", result[0]["bar"]);
        return;

        bool Match(Row left, Row right) => Equals(left["bar_id"], right["id"]);
    }
    
    [Fact]
    public void Sandbox2()
    {
        var logger = new Mock<ILogger<TestJoinOp>>();
        var loggerFactory = new Mock<ILoggerFactory>();
        
        var obj1 = new { id = 123, foo = "bar" };
        var obj2 = new { id = 456, foo_id = 123, bar = "quux" };

        var op = new TestJoinOp(Match, logger.Object);
        op.Left(new TestExtract(logger.Object, obj1)); 
        op.Right(new TestExtract(logger.Object, obj2));
        
        var pipelineExecutor = 
            new SingleThreadedPipelineExecutor(loggerFactory.Object);

        // Usually the pipeline executor is set by the ETL process hosting
        // the operation, but since we have no process here, we need to prepare
        // the operation manually.
        op.Prepare(pipelineExecutor);
        
        var result = op.Execute([]).ToList();
        
        Assert.Single(result);
        Assert.Equal("quux", result[0]["bar"]);
        return;

        bool Match(Row left, Row right) => Equals(left["id"], right["foo_id"]);
    }
}