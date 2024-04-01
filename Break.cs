namespace TucanScript.Core;

public class Break : IExecutable
{
    private readonly ILoop? _loop;

    public Break(ILoop? loop)
    {
        _loop = loop;
    }

    public void Execute() => _loop?.Break();
    
    public void Reset() => throw new Exception("Not resetable object");
}