namespace TucanScript.Core;

public class Undefined : Entity
{
    public Undefined(String content)
    {
        Content = content;
        Type = EntityType.Undefined;
    }
    
    public String Content { get; }
}