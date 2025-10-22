using Microsoft.Extensions.Logging;

namespace Setl.Cmd.Examples.AowAio;

public class ExampleProcess : EtlProcess
{
    private readonly ILogger logger;
    
    public ExampleProcess(
        IPipelineExecutor pipelineExecutor, 
        ILogger logger) 
        : base(pipelineExecutor, logger)
    {
        this.logger = logger;
    }

    protected override void Initialize()
    {
        this.Register(new ExtractLinesOperation(this.logger));
        
        this.Register(new WriteLines(this.logger));
    }
}