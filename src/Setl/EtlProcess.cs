using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class EtlProcess : EtlProcessBase<EtlProcess>
{
    protected EtlProcess(ILogger logger) 
        : base(logger)
    {
    }
}