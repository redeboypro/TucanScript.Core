namespace TucanScript.Core;

public class Entity
{
    public static readonly Entity None = new (EntityType.None);

    protected Entity()
    {
        Type = EntityType.None;
    }
    
    public Entity(EntityType type)
    {
        Type = type;
    }
    
    public EntityType Type { get; protected set; }
}