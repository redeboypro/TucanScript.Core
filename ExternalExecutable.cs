namespace TucanScript.Core;

public class ExternalExecutable : IExecutable
{
    private readonly Function _function;
    
    public ExternalExecutable(Function function)
    {
        _function = function;
    }

    public Action<Function>? Delegate { get; init; }
    
    public void Execute() => Delegate?.Invoke(_function);
    
    public void Reset() => throw new Exception("Not resetable object");
}