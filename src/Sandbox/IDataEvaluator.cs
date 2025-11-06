using Sandbox.Support;

namespace Sandbox;

internal interface IDataEvaluator
{
    Row Evaluate(Dictionary<string, string> data);
    
    bool TryEvaluate(Dictionary<string, string> data, out Row result);
}