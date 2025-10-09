using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Setl.Cmd.Avd.Operations;

namespace Setl.Cmd.Avd;

public class AvdExampleEtlProcess : EtlProcess
{
    private readonly StreamReader reader;
    private readonly ILogger logger;

    public static Action<int, string, string> OnParseError { get; set; } 
        = (_, _, _) => { };
    
    public AvdExampleEtlProcess(
        StreamReader reader,
        IPipelineExecutor pipelineExecutor,
        ILogger logger)
        : base(pipelineExecutor, logger)
    {
        this.reader = reader;
        this.logger = logger;
    }

    protected override void Initialize()
    {
        var parser = AvdExampleEtlProcess.CreateParser();
        
        var extract = new ExtractAvdRecordsOperation(this.reader, parser, this.logger);
        var load = new WriteAvdRecordsOperation(this.logger);
        var pad = new PadFieldsOperation(this.logger)
        {
            OnPaddingAdded = (index, field, orig, dest, source) =>
            {
                this.logger.LogInformation(
                    "Padding added to {Field} in {Source} (index {Index}, ['{Orig}' -> '{Dest}'])",
                    field,
                    source,
                    index,
                    orig,
                    dest);
            },
        };

        var validateBsn = new ValidateBsnOperation(this.logger)
        {
            OnInvalidBsn = (index, bsn, source) =>
            {
                this.logger.LogWarning(
                    "Invalid BSN (11-proef) '{Bsn}' in {Source} (index {Index})",
                    bsn,
                    source,
                    index);
            },
        };

        var validateGemeente = new ValidateGemeenteOperation(this.logger)
        {
            OnInvalidGemeentecode = (
                index, gemeentecode, source) =>
            {
                this.logger.LogWarning(
                    "Invalid Gemeentecode {Gemeentecode} in {Source} (index {Index})",
                    gemeentecode,
                    source,
                    index);
            },       
        };
        
        this.Register(extract);
        this.Register(pad);
        this.Register(validateBsn);
        this.Register(validateGemeente);
        this.Register(load);
    }

    private static Parser CreateParser() =>
        new()
        {
            OnHeaderParseError = (i, s) =>
            {
                AvdExampleEtlProcess.OnParseError(i, "Invalid header", s);
            },
            OnRecordParseError = (i, s) =>
            {
                AvdExampleEtlProcess.OnParseError(i, "Invalid record", s);
            },
        };
}