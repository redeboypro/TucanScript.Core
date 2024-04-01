namespace TucanScript.Core;

public class For : Statement, ILoop
{
    private readonly IReadOnlyList<Operable?> _operables;
    private readonly Expression _arrayExpression;
    private bool _inProgress;
    private bool _isToContinue;

    public For(IReadOnlyList<Operable?> operables, Expression arrayExpression)
    {
        _operables = operables;
        _arrayExpression = arrayExpression;
    }
    
    public override void Execute()
    {
        _inProgress = true;
        _arrayExpression.Execute();
        foreach (var element in _arrayExpression)
        {
            foreach (var operable in _operables)
                operable?.Set(element);

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
        }
    }

    public void Break() => _inProgress = false;
    
    public void Continue() =>_isToContinue = true;
}