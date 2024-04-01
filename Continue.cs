namespace TucanScript.Core;

public class Continue : IExecutable
{
    public ILoop? _loop;

    public Continue(ILoop? loop)
    {
        _loop = loop;
    }

    public void Execute() => _loop?.Continue();

    public void Reset() => throw new Exception("Not resetable object");
}