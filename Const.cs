namespace TucanScript.Core;

public class Const : Operable, IResetable
{
    private EntityType _originType;
    private String? _originStringValue;
    private Int64 _originIntValue;
    private Double _originFloatingPointValue;
    private Boolean _originBooleanValue;

    public Const(String? originStringValue)
    {
        _originType = EntityType.String;
        _originStringValue = originStringValue;
        Reset();
    }
    
    public Const(Int64 originIntValue)
    {
        _originType = EntityType.Integer;
        _originIntValue = originIntValue;
        Reset();
    }
    
    public Const(Double originFloatingPointValue)
    {
        _originType = EntityType.FloatingPoint;
        _originFloatingPointValue = originFloatingPointValue;
        Reset();
    }
    
    public Const(Boolean originBooleanValue)
    {
        _originType = EntityType.Boolean;
        _originBooleanValue = originBooleanValue;
        Reset();
    }

    public void Reset()
    {
        Set(_originType,
            _originStringValue,
            _originIntValue,
            _originFloatingPointValue,
            _originBooleanValue, null);
    }

    protected override void OnReleaseUnmanaged()
    {
        _originType = 0;
        _originStringValue = null;
        _originIntValue = 0;
        _originFloatingPointValue = 0.0;
        _originBooleanValue = false;
    }
}