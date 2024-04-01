namespace TucanScript.Core;

public class Return : IExecutable
{
    private readonly IReturnable _returnable;

    public Return(IReturnable function)
    {
        _returnable = function;
    }

    public void Execute()
    {
        _returnable.Return();
    }
    
    public void Reset() => throw new Exception("Not resetable object");
}