using System.Collections;
using System.Globalization;
using System.Text;

namespace TucanScript.Core;

public class Operable : Entity, IDisposable, IReadOnlyList<Operable?>
{
    private readonly List<Operable?> _arrayList;

    private String? _stringValue;
    private Int64 _intValue;
    private Double _floatingPointValue;
    private Boolean _booleanValue;

    public Operable(EntityType type = EntityType.Integer)
    {
        _arrayList = new List<Operable?>();
        Type = type;
    }

    public String? GetStringValue()
    {
        return _stringValue;
    }

    public Int64 GetIntValue()
    {
        return _intValue;
    }

    public Double GetFloatingPointValue()
    {
        return _floatingPointValue;
    }
    
    public Boolean GetBooleanValue()
    {
        return _booleanValue;
    }

    public void Set(Operable rValue)
    {
        Type = rValue.Type;
        _stringValue = rValue._stringValue;
        _intValue = rValue._intValue;
        _floatingPointValue = rValue._floatingPointValue;
        _booleanValue = rValue._booleanValue;
        
        _arrayList.Clear();
        for (var i = 0; i < rValue.Count; i++)
            this[i] = rValue[i];
    }

    public void Set(String? rValue)
    {
        Set(EntityType.String, rValue, 0, 0.0, false, null);
        _arrayList.Clear();
    }
    
    public void Set(Int64 rValue)
    {
        Set(EntityType.Integer, null, rValue, 0.0, false, null);
        _arrayList.Clear();
    }
    
    public void Set(Double rValue)
    {
        Set(EntityType.FloatingPoint, null, 0, rValue, false, null);
        _arrayList.Clear();
    }
    
    public void Set(Boolean rValue)
    {
        Set(EntityType.Boolean, null, 0, 0.0, rValue, null);
        _arrayList.Clear();
    }
    
    public void Set(IReadOnlyList<Operable> rValue)
    {
        Set(EntityType.Array, null, 0, 0.0, false, rValue);
        _arrayList.Clear();
    }

    protected void Set(EntityType type, string? stringValue, Int64 intValue, Double floatingPointValue, Boolean booleanValue, IReadOnlyList<Operable>? arrayList)
    {
        Type = type;
        _stringValue = stringValue;
        _intValue = intValue;
        _floatingPointValue = floatingPointValue;
        _booleanValue = booleanValue;
        
        if (type != EntityType.Array)
            _arrayList.Clear();

        if (arrayList == null) 
            return;

        _arrayList.Clear();
        for (var i = 0; i < _arrayList.Count; i++)
            _arrayList[i] = arrayList[i];
    }

    public void Add(String? rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.String;
                _stringValue = _intValue + rValue;
                _intValue = 0;
                break;
            case EntityType.String:
                _stringValue += rValue;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.String;
                _stringValue = _intValue + rValue;
                _floatingPointValue = 0;
                break;
            case EntityType.Boolean:
                Type = EntityType.String;
                _stringValue = _booleanValue + rValue;
                _booleanValue = false;
                break;
        }
    }
    
    public void Add(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                _intValue += rValue;
                break;
            case EntityType.String:
                _stringValue += rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue += rValue;
                break;
        }
    }
    
    public void Add(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.FloatingPoint;
                _floatingPointValue = _intValue + rValue;
                _intValue = 0;
                break;
            case EntityType.String:
                _stringValue += rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue += rValue;
                break;
        }
    }

    public void Add(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.String:
                Add(rValue.GetStringValue());
                break;
            case EntityType.Integer:
                Add(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Add(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void Sub(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                _intValue -= rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue -= rValue;
                break;
        }
    }
    
    public void Sub(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.FloatingPoint;
                _floatingPointValue = _intValue - rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue -= rValue;
                break;
        }
    }
    
    public void Sub(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Sub(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Sub(rValue.GetFloatingPointValue());
                break;
        }
    }

    public void Mul(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                _intValue *= rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue *= rValue;
                break;
        }
    }
    
    public void Mul(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.FloatingPoint;
                _floatingPointValue = _intValue * rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue *= rValue;
                break;
        }
    }
    
    public void Mul(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Mul(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Mul(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void Div(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                _intValue /= rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue /= rValue;
                break;
        }
    }
    
    public void Div(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.FloatingPoint;
                _floatingPointValue = _intValue / rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue /= rValue;
                break;
        }
    }
    
    public void Div(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Div(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Div(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void Rem(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                _intValue %= rValue;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue %= rValue;
                break;
        }
    }
    
    public void Rem(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.FloatingPoint;
                _floatingPointValue = _intValue % rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                _floatingPointValue %= rValue;
                break;
        }
    }
    
    public void Rem(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Rem(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Rem(rValue.GetFloatingPointValue());
                break;
        }
    }

    public void Equal(string? rValue)
    {
        switch (Type)
        {
            case EntityType.String:
                Type = EntityType.Boolean;
                _booleanValue = _stringValue == rValue;
                _stringValue = null;
                break;
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue.ToString() == rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue
                    .ToString(CultureInfo.InvariantCulture) == rValue;
                _floatingPointValue = 0.0;
                break;
            case EntityType.Boolean:
                _booleanValue = _booleanValue.ToString() == rValue;
                break;
        }
    }
    
    public void Equal(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.String:
                Type = EntityType.Boolean;
                _booleanValue = _stringValue == rValue.ToString();
                _stringValue = null;
                break;
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue == rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = Math.Abs(_floatingPointValue - rValue) < Double.Epsilon;
                _floatingPointValue = 0.0;
                break;
            case EntityType.Boolean:
                _booleanValue = _booleanValue == Convert.ToBoolean(rValue);
                break;
        }
    }
    
    public void Equal(Double rValue)
    {
        switch (Type)
        {
            case EntityType.String:
                Type = EntityType.Boolean;
                _booleanValue = _stringValue == rValue.ToString(CultureInfo.InvariantCulture);
                _stringValue = null;
                break;
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = Math.Abs(_intValue - rValue) < Double.Epsilon;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = Math.Abs(_floatingPointValue - rValue) < Double.Epsilon;
                _floatingPointValue = 0.0;
                break;
            case EntityType.Boolean:
                _booleanValue = _booleanValue == Convert.ToBoolean(rValue);
                break;
        }
    }
    
    public void Equal(Boolean rValue)
    {
        switch (Type)
        {
            case EntityType.String:
                Type = EntityType.Boolean;
                _booleanValue = _stringValue == rValue.ToString();
                _stringValue = null;
                break;
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = Convert.ToBoolean(_intValue) == rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = Convert.ToBoolean(_floatingPointValue) == rValue;
                _floatingPointValue = 0.0;
                break;
            case EntityType.Boolean:
                _booleanValue = _booleanValue == rValue;
                break;
        }
    }
    
    public void Equal(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.String:
                Equal(rValue.GetStringValue());
                break;
            case EntityType.Integer:
                Equal(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Equal(rValue.GetFloatingPointValue());
                break;
            case EntityType.Boolean:
                Equal(rValue.GetBooleanValue());
                break;
            case EntityType.Array:
                var array = rValue._arrayList;
                
                break;
        }
    }

    public void Greater(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue > rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue > rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void Greater(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue > rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue > rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void Greater(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Greater(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Greater(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void Less(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue < rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue < rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void Less(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue < rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue < rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void Less(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                Less(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                Less(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void GEqual(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue >= rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue >= rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void GEqual(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue >= rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue >= rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void GEqual(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                GEqual(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                GEqual(rValue.GetFloatingPointValue());
                break;
        }
    }
    
    public void LEqual(Int64 rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue <= rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue <= rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void LEqual(Double rValue)
    {
        switch (Type)
        {
            case EntityType.Integer:
                Type = EntityType.Boolean;
                _booleanValue = _intValue <= rValue;
                _intValue = 0;
                break;
            case EntityType.FloatingPoint:
                Type = EntityType.Boolean;
                _booleanValue = _floatingPointValue <= rValue;
                _floatingPointValue = 0;
                break;
        }
    }
    
    public void LEqual(Operable rValue)
    {
        switch (rValue.Type)
        {
            case EntityType.Integer:
                LEqual(rValue.GetIntValue());
                break;
            case EntityType.FloatingPoint:
                LEqual(rValue.GetFloatingPointValue());
                break;
        }
    }

    public void And(Operable rValue)
    {
        if (Type == EntityType.Boolean) 
            _booleanValue = _booleanValue && rValue.GetBooleanValue();
    }
    
    public void Or(Operable rValue)
    {
        if (Type == EntityType.Boolean) 
            _booleanValue = _booleanValue || rValue.GetBooleanValue();
    }

    public Int16 GetInt16()
    {
        return (Int16) _intValue;
    }
    
    public Int32 GetInt32()
    {
        return (Int32) _intValue;
    }
    
    public UInt16 GetUInt16()
    {
        return (UInt16) _intValue;
    }
    
    public UInt32 GetUInt32()
    {
        return (UInt32) _intValue;
    }
    
    public UInt64 GetUInt64()
    {
        return (UInt64) _intValue;
    }

    public Single GetSingle()
    {
        return (Single) _floatingPointValue;
    }

    public object? GetNativeObject()
    {
        switch (Type)
        {
            case EntityType.String:
                return GetStringValue();
            case EntityType.Integer:
                return GetIntValue();
            case EntityType.FloatingPoint:
                return GetFloatingPointValue();
            case EntityType.Boolean:
                return GetBooleanValue();
            case EntityType.Array:
                var arrayList = new object?[Count];
                for (var i = 0; i < Count; i++)
                    arrayList[i] = _arrayList[i]?.GetNativeObject();
                return arrayList;
        }

        return null;
    }

    public override String ToString()
    {
        switch (Type)
        {
            case EntityType.String:
                return GetStringValue() ?? String.Empty;
            case EntityType.Integer:
                return GetIntValue().ToString();
            case EntityType.FloatingPoint:
                return GetFloatingPointValue().ToString(CultureInfo.InvariantCulture);
            case EntityType.Boolean:
                return GetBooleanValue().ToString();
            case EntityType.Array:
                var formatted = new StringBuilder();
                formatted.Append('{');
                for (var i = 0; i < _arrayList.Count; i++)
                {
                    formatted.Append(_arrayList[i]);
                    
                    if (i < _arrayList.Count - 1)
                        formatted.Append(", ");
                }
                formatted.Append('}');
                return formatted.ToString();
        }
        
        return String.Empty;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Release()
    {
        _stringValue = null;
        _intValue = 0;
        _floatingPointValue = 0.0;
        _booleanValue = false;
        OnReleaseUnmanaged();
    }

    protected virtual void OnReleaseUnmanaged() { }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    public IEnumerator<Operable> GetEnumerator()
    {
        return _arrayList.GetEnumerator();
    }

    ~Operable()
    {
        Release();
    }

    public int Count
    {
        get
        {
            return _arrayList.Count;
        }
    }

    public Operable? this[int index]
    {
        get
        {
            return _arrayList[index];
        }
        set
        {
            Set(EntityType.Array, null, 0, 0.0, false, null);
            
            var count = Count;
            
            if (index >= count)
                for (var i = 0; i < count - index + 1; i++)
                    _arrayList.Add(new Operable());

            if (value != null) 
                _arrayList[index]?.Set(value);
        }
    }

    public Operable Clone()
    {
        var operable = new Operable();
        operable.Set(this);
        return operable;
    }
}