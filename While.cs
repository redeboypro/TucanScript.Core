namespace TucanScript.Core;

public class While : Statement, ILoop
{
    private readonly Expression _conditionExpression;
    private bool _inProgress;
    private bool _isToContinue;

    public While(Expression conditionExpression)
    {
        _conditionExpression = conditionExpression;
    }
    
    public override void Execute()
    {
        _inProgress = true;
        _conditionExpression.Execute();
        while (_conditionExpression.GetBooleanValue())
        {
            foreach (var executable in Executables)
            {
                executable.Execute();
                
                if (_isToContinue)
                {
                    _isToContinue = false;
                    continue;
                }
                
                if (!_inProgress)
                    return;
            }

            _conditionExpression.Execute();
        }
    }

    public void Break() => _inProgress = false;

    public void Continue() => _isToContinue = true;
}