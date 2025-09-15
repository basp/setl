using Microsoft.Extensions.Logging;

namespace Setl;

public class SimpleProcess : EtlProcess
{
    private readonly Action<EtlProcess> initialize;

    public SimpleProcess(
        Action<EtlProcess> initialize,
        ILogger logger,
        IPipelineExecutor pipelineExecutor)
        : base(logger, pipelineExecutor)
    {
        this.initialize = initialize;
    }
    
    protected override void Initialize()
    {
        this.initialize(this);
    }
}