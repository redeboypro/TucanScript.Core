namespace TucanScript.Core;

public class OperableClone : Operable, IResetable
{
    private Operable? _source;

    public OperableClone(Operable? source)
    {
        _source = source;
    }

    public void SetSource(Operable? operable)
    {
        _source = operable;
    }

    public void Reset()
    {
        if (_source == null)
            return;
        
        Set(_source);
    }

    public void Apply()
    {
        _source?.Set(this);
    }
}