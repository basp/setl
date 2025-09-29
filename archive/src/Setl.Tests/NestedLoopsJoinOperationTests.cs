using Microsoft.Extensions.Logging;
using Moq;
using Setl.Operations;

namespace Setl.Tests;

public class NestedLoopsJoinOperationTests
{
    public class TestJoinOp : NestedLoopsJoinOperation
    {
        public TestJoinOp(ILogger logger) : base(logger)
        {
        }

        protected override Row MergeRows(Row leftRow, Row rightRow)
        {
            var merged = leftRow.Clone();
            merged["bar"] = rightRow["bar"];
            return merged;
        }

        protected override bool MatchJoinCondition(Row leftRow, Row rightRow) =>
            leftRow["id"] == rightRow["foo_id"];
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
            rows.Select(Row.FromObject);
    }
    
    [Fact]
    public void Sandbox()
    {
        var logger = new Mock<ILogger<TestJoinOp>>();
        var op = new TestJoinOp(logger.Object);
        var obj1 = new { id = 123, foo = "bar" };
        var obj2 = new { id = 456, foo_id = 123, bar = "quux" };
        op.Left(new TestExtract(logger.Object, obj1));
        op.Right(new TestExtract(logger.Object, obj2));
        var result = op.Execute([]).ToList();
        Assert.Single(result);
        Assert.Equal("quux", result[0]["bar"]);
    }
}