using Microsoft.Extensions.Logging;
using Moq;
using Setl.Operations;

namespace Setl.Tests;

public class NestedLoopsJoinOperationTests
{
    // For this set of tests:
    // Usually the pipeline executor is set by the ETL process hosting
    // the operation, but since we have no process here, we need to prepare
    // the operation manually.

    // ReSharper disable once MemberCanBePrivate.Global
    public class TestJoinOp : NestedLoopsJoinOperation
    {
        private readonly Func<Row, Row, Row> merge;
        private readonly Func<Row, Row, bool> match;

        public TestJoinOp(
            Func<Row, Row, Row> merge,
            Func<Row, Row, bool> match,
            ILogger logger) 
            : base(logger)
        {
            this.merge = merge;
            this.match = match;
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) =>
            this.merge(leftRow, rightRow);

        protected override bool MatchJoinCondition(Row leftRow, Row rightRow)
        {
            leftRow.MissingKeyBehavior = MissingKeyBehavior.Ignore;
            rightRow.MissingKeyBehavior = MissingKeyBehavior.Ignore;
            return this.match(leftRow, rightRow);
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
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
    public void LeftOrientedJoin()
    {
        var logger = new Mock<ILogger<TestJoinOp>>();
        var loggerFactory = new Mock<ILoggerFactory>();
        
        var obj1 = new { id = 123, bar_id = 456, foo = "bar" };
        var obj2 = new { id = 456, bar = "quux" };

        var op = new TestJoinOp(Merge, Match, logger.Object);
        op.Left(new TestExtract(logger.Object, obj1)); 
        op.Right(new TestExtract(logger.Object, obj2));
        
        var pipelineExecutor = 
            new SingleThreadedPipelineExecutor(loggerFactory.Object);

        op.Prepare(pipelineExecutor);
        
        var result = op.Execute([]).ToList();
        
        Assert.Single(result);
        Assert.Equal("quux", result[0]["bar"]);
        
        return;

        Row Merge(Row left, Row right)
        {
            var merged = left.Clone();
            merged["bar"] = right["bar"];
            return merged;
        }

        bool Match(Row left, Row right) => Equals(left["bar_id"], right["id"]);
    }
    
    [Fact]
    public void RightOrientedJoin()
    {
        var logger = new Mock<ILogger<TestJoinOp>>();
        var loggerFactory = new Mock<ILoggerFactory>();
        
        var obj1 = new { id = 123, foo = "bar" };
        var obj2 = new { id = 456, foo_id = 123, bar = "quux" };

        var op = new TestJoinOp(Merge, Match, logger.Object);
        op.Left(new TestExtract(logger.Object, obj1)); 
        op.Right(new TestExtract(logger.Object, obj2));
        
        var pipelineExecutor = 
            new SingleThreadedPipelineExecutor(loggerFactory.Object);

        op.Prepare(pipelineExecutor);
        
        var result = op.Execute([]).ToList();
        
        Assert.Single(result);
        Assert.Equal("quux", result[0]["bar"]);
        
        return;

        Row Merge(Row left, Row right)
        {
            var merged = left.Clone();
            merged["bar"] = right["bar"];
            return merged;
        }
        
        bool Match(Row left, Row right) => Equals(left["id"], right["foo_id"]);
    }

    [Fact]
    public void SimpleInnerJoinExample()
    {
        object?[] foos =
        [
            new
            {
                id = 1,
                name = "foo1",
            },
            new
            {
                id = 2,
                name = "foo2",
            }
        ];

        object?[] bars =
        [
            new
            {
                id = 1,
                foo_id = 1,
                name = "foo1:bar1",
            },
            new
            {
                id = 2,
                foo_id = 1,
                name = "foo1:bar2",
            },
            new
            {
                id = 3,
                foo_id = 1,
                name = "foo1:bar3",
            },
            new
            {
                id = 4,
                foo_id = 2,
                name = "foo2:bar4",
            },
            new
            {
                id = 5,
                foo_id = 2,
                name = "foo2:bar5",
            }
        ];
        
        var logger = new Mock<ILogger<TestJoinOp>>();
        var loggerFactory = new Mock<ILoggerFactory>();

        var op = new TestJoinOp(Merge, Match, logger.Object);
        op.Left(new TestExtract(logger.Object, foos)); 
        op.Right(new TestExtract(logger.Object, bars));
        
        var pipelineExecutor = 
            new SingleThreadedPipelineExecutor(loggerFactory.Object);

        op.Prepare(pipelineExecutor);
        
        var result = op.Execute([]).ToList();
        Assert.Equal(bars.Length, result.Count);
        Assert.Equal("foo1:bar1", result[0]["bar_name"]);
        
        return;

        Row Merge(Row left, Row right)
        {
            var merged = left.Clone();
            merged["bar_id"] = right["id"];
            merged["bar_name"] = right["name"];
            return merged;
        }

        bool Match(Row left, Row right)
        {
            left.MissingKeyBehavior = MissingKeyBehavior.Ignore;
            right.MissingKeyBehavior = MissingKeyBehavior.Ignore;
            return Equals(left["id"], right["foo_id"]);
        }
    }
}