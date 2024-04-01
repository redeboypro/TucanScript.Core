namespace TucanScript.Core;

public class Argument
{
    public bool IsReference { get; set; }
    public OperableClone? Variable { get; init; }
}