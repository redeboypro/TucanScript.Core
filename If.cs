namespace TucanScript.Core;

public class If : Statement
{
    private readonly Expression _conditionExpression;

    public If(Expression conditionExpression)
    {
        _conditionExpression = conditionExpression;
    }
    
    public override void Execute()
    {
        _conditionExpression.Execute();

        if (!_conditionExpression.GetBooleanValue()) 
            return;

        foreach (var executable in Executables)
            executable.Execute();
    }
}